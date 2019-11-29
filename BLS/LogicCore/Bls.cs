using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ChangeTracking;
// ReSharper disable InvalidXmlDocComment

[assembly: InternalsVisibleTo("BLS.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace BLS
{
    public enum BlOperator
    {
        And, Or, Eq, NotEq, Grt, Ls, GrtOrEq, LsOrEq
    }

    public class BlBinaryExpression
    {
        public BlBinaryExpression Right { get; set; }
        public BlOperator Operator { get; set; }
        public BlBinaryExpression Left { get; set; }

        public bool IsLeaf { get; set; }
        public string PropName { get; set; }
        public object Value { get; set; }
    }

    public class Bls
    {
        private IBlGraph _graph;
        private IBlStorageProvider _storageProvider;

        private List<BlsPawn> _toAdd = new List<BlsPawn>();
        private List<Connection> _toConnect = new List<Connection>();
        private List<Connection> _toDisconnect = new List<Connection>();
        private List<BlsPawn> _toRemove = new List<BlsPawn>();
        private List<BlsPawn> _toUpdate = new List<BlsPawn>();
        private string ConstantExpressionType = "ConstantExpression";
        private string LogicalBinaryExpression = "LogicalBinaryExpression";

        private string MethodBinaryExpression = "MethodBinaryExpression";

        // dependency on internal Expression implementation as these types are internal and are subject to changes
        private string PropertyExpressionType = "PropertyExpression";

        /// <summary>
        /// Use this constructor to create a new instance of the application's business logic.
        /// You'll have to provide an instance of a storage provider - a class which implements
        /// <see cref="IBlStorageProvider"/> interface. Storage providers are used by BLS to interact with
        /// databases or other storage solutions.
        /// </summary>
        /// <param name="storageProvider">Storage provider</param>
        public Bls(IBlStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
            _graph = new BlGraph();
        }

        /// <summary>
        /// Use this constructor for manually overriding the instance of the <see cref="IBlGraph"/> interface which,
        /// by default, is bound to an instance of <see cref="BlGraph"/> class. This is an internal constructor;
        /// it should stay this way (there is no reason for the consumer to work with the BlGraph) and only used
        /// for testing purposes.
        /// </summary>
        /// <param name="storageProvider"></param>
        /// <param name="graph"></param>
        internal Bls(IBlStorageProvider storageProvider, IBlGraph graph)
        {
            _storageProvider = storageProvider;
            _graph = graph;
        }

        /// <summary>
        /// Call this method to register your BLS pawns. 
        /// </summary>
        /// <param name="pawns">List of instances of pawns to register</param>
        /// <example>
        /// <code>
        /// public class SomePawn : BlsPawn {}
        /// public class SomeOtherPawn : BlsPawn {}
        /// Bls bls = new Bls(instance_of_storage_provider);
        /// bls.RegisterBlPawns(new SomePawn(), new SomeOtherPawn())
        /// </code>
        /// </example>
        public void RegisterBlPawns(params BlsPawn[] pawns)
        {
            if (_graph == null)
            {
                _graph = new BlGraph();
            }
            _graph.RegisterPawns(pawns);
            _graph.CompileGraph();
        }

        /// <summary>
        /// Call this method to spawn a new instance of a pawn. Remember to always create new BLS pawns
        /// using this call and not the pawn's default constructor (new Pawn())
        /// </summary>
        /// <typeparam name="TPawn">Type of the pawn</typeparam>
        /// <returns>Instance of the pawn</returns>
        /// <exception cref="BlGraphNotRegisteredError"></exception>
        /// <exception cref="PawnNotRegisteredError"></exception>
        /// <remarks>Newly spawned pawns will NOT save in storage unless you call the
        /// <see cref="Bls.PersistChanges()"/> method
        /// </remarks>
        public TPawn SpawnNew<TPawn>() where TPawn : BlsPawn, new()
        {
            if (_graph == null)
            {
                throw new BlGraphNotRegisteredError("BlGraph is not initialized");
            }
            if (_graph != null && (_graph.CompiledCollections == null || _graph.CompiledCollections.Count < 1))
            {
                throw new PawnNotRegisteredError(typeof(TPawn).Name);
            }

            if (_graph != null)
            {
                var registeredPawns = _graph.CompiledCollections.Select(c => c.BlContainerName).ToArray();
                if (registeredPawns.All(p => p != typeof(TPawn).Name))
                {
                    throw new PawnNotRegisteredError(typeof(TPawn).Name);
                }
            }

            var pawn = new TPawn
            {
                SystemRef = this,
                Created = DateTime.Now,
                LastTimeModified = DateTime.Now
            };
            var traceablePawn = pawn.AsTrackable();
            _toAdd.Add(traceablePawn);
            return traceablePawn;
        }

        /// <summary>
        /// Use this method to find pawns based on their properties. If you do not provide any
        /// filter predicates, the method will return all pawns of the specified type.
        /// </summary>
        /// <param name="includeSoftDeleted">Also retrieve soft deleted pawns if set to true; false by default</param>
        /// <param name="filter">Boolean expression specifying the filtering conditions</param>
        /// <param name="sortProperty">Property of the pawn to use for sorting</param>
        /// <param name="sortDir">Ascending or Descending</param>
        /// <param name="batchSize">Number of records to return in the <see cref="BLS.StorageCursor"/> class></param>
        /// <typeparam name="TPawn">Type of the pawn</typeparam>
        /// <returns>Storage Cursor containing the resulting collection of pawns</returns>
        /// <exception cref="NotImplementedException"></exception>
        public StorageCursor<TPawn> Find<TPawn>(
            bool includeSoftDeleted = false,
            Expression<Func<TPawn, bool>> filter = null,
            Expression<Func<TPawn, object>> sortProperty = null,
            Sort sortDir = Sort.Asc,
            int batchSize = 200) where TPawn: BlsPawn, new()
        {
            var container = _graph.GetStorageContainerNameForPawn(new TPawn());
            BlBinaryExpression filterExpression = filter == null ? null : ResolveFilterExpression(filter);
            string sortProp = sortProperty == null ? null : ResolveSortExpression(sortProperty);
            
            return _storageProvider.FindInContainer<TPawn>(container, filterExpression, sortProp, sortDir.ToString());
        }

        /// <summary>
        /// Call the method to search for pawns based on the search term, optionally specifying
        /// any additional filters.
        /// </summary>
        /// <param name="searchTerm">The term to search</param>
        /// <param name="filter">Boolean expression specifying any additional filter conditions</param>
        /// <param name="includeSoftDeleted">Also retrieve soft deleted pawns if set to true; false by default</param>
        /// <param name="sortProperty">Property of the pawn to use for sorting</param>
        /// <param name="sortDir">Ascending or Descending</param>
        /// <param name="batchSize">Number of records to return in the <see cref="BLS.StorageCursor"/> class</param>
        /// <param name="searchProperties">List of property expressions to apply the search on. Only properties
        /// marked with the <see cref="BLS.Functional.FullTextSearchable"/> attribute are allowed in the list of</param>
        /// <typeparam name="TPawn">Type of the pawn</typeparam>
        /// <returns>Storage Cursor containing the resulting collection of pawns</returns>
        /// <exception cref="NotImplementedException"></exception>
        public StorageCursor<TPawn> Search<TPawn>(
            string searchTerm,
            bool includeSoftDeleted = false,
            Expression<Func<TPawn, bool>> filter = null,
            Expression<Func<TPawn, object>> sortProperty = null,
            Sort sortDir = Sort.Asc,
            int batchSize = 200,
            params Expression<Func<TPawn, string>>[] searchProperties) where TPawn: BlsPawn, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call this method to retrieve a pawn with the specified ID
        /// </summary>
        /// <param name="id">ID of the stored pawn</param>
        /// <typeparam name="T">Type of the pawn</typeparam>
        /// <returns>Pawn found in the storage or <c>null</c> if no pawn is found</returns>
        /// <exception cref="NotImplementedException"></exception>
        public T GetById<T>(string id) where T : BlsPawn, new()
        {
            var container = _graph.GetStorageContainerNameForPawn(new T());
            if (_graph.CompiledCollections.Select(c => c.StorageContainerName).Any(c => c == container))
            {
                return _storageProvider.GetById<T>(id, container);
            }

            throw new PawnNotRegisteredError($"Pawn of type {typeof(T).Name} is not registered in BLS");
        }

        /// <summary>
        /// Use this method to call a query directly on the storage using whatever query language the
        /// storage system supports. This is a fallback method; please use it with caution as queries may become very difficult to
        /// read very quickly. A justified example of using this method would be when you need to
        /// perform a resource-intensive computation and it makes sense to do it on the storage server.
        /// Otherwise, BLS should provide all necessary machinery to manipulate both im-memory and in-storage
        /// data without too much hassle.
        /// </summary>
        /// <param name="query">Query to perform; the query is not verified and sent directly to the storage server</param>
        /// <typeparam name="T">Type of the result object; does not have to be a pawn</typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public StorageCursor<T> GetByQuery<T>(string query) where T: new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call this method to delete a pawn
        /// </summary>
        /// <param name="pawn">Pawn to delete</param>
        /// <typeparam name="TPawn">Type of the pawn</typeparam>
        /// <remarks>Deletions will NOT be saved in storage unless you call the
        /// <see cref="PersistChanges()"/> method.
        /// </remarks>
        public void Delete<TPawn>(TPawn pawn) where TPawn: BlsPawn
        {
            _toConnect.RemoveAll(connection => connection.From == pawn || connection.To == pawn);
            _toDisconnect.RemoveAll(connection => connection.From == pawn || connection.To == pawn);
            if (_toAdd.Contains(pawn))
            {
                _toAdd.Remove(pawn);
            }
            else
            {
                _toRemove.Add(pawn);
            }
        }

        /// <summary>
        /// Call this method to persist changes. BLS will try to save all model updates
        /// done earlier in one all-or-nothing transaction. If there are no errors, <c>void</c> will return;
        /// otherwise an exception will be thrown.
        /// </summary>
        /// <exception cref="NoStorageProviderRegisteredError">Thrown if there is no storage provider registered</exception>
        public void PersistChanges()
        {
            if (_storageProvider == null)
            {
                throw new NoStorageProviderRegisteredError();
            }
        }

        [ExcludeFromCodeCoverage] // pass through
        public void OverrideStorageNamingEncoder(IStorageNamingEncoder encoder)
        {
            _graph.OverrideStorageNamingEncoder(encoder);
        }

        [ExcludeFromCodeCoverage] // pass through
        internal void Connect(BlsPawn source, BlsPawn target, string relation)
        {
            _toConnect.Add(new Connection {From = source, To = target, RelationName = relation});
        }

        [ExcludeFromCodeCoverage] // pass through
        internal void Disconnect(BlsPawn source, BlsPawn target, string relation)
        {
            _toDisconnect.Add(new Connection {From = source, To = target, RelationName = relation});
        }

        private BlBinaryExpression ResolveFilterExpression<TPawn>(Expression<Func<TPawn, bool>> filter) where TPawn : BlsPawn, new()
        {
            if (!(filter.Body is BinaryExpression expression))
            {
                throw new IncorrectFilterArgumentStructureError($"Filter expression has to be a binary expression. You provided {filter.Body.Type}");
            }

            if (expression.GetType().Name == MethodBinaryExpression)
            {
                return ResolveComparisonFilterExpression(expression);
            }

            if (expression.GetType().Name == LogicalBinaryExpression)
            {
                return ResolveBinaryFilterExpression(expression, new BlBinaryExpression());
            }

            throw new IncorrectFilterArgumentStructureError($"Filter expression has to be a binary expression. You provided {filter.Body.Type}");
        }

        // recursive method to resolve AND/OR binary filter expressions
        private BlBinaryExpression ResolveBinaryFilterExpression(BinaryExpression expression, BlBinaryExpression newExpression)
        {
            newExpression.Left = new BlBinaryExpression();
            newExpression.Right = new BlBinaryExpression();
            
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                {
                    newExpression.Operator = BlOperator.And;
                    break;
                }
                case ExpressionType.OrElse:
                {
                    newExpression.Operator = BlOperator.Or;
                    break;
                }
            }
            
            if (expression.Left.GetType().Name == MethodBinaryExpression)
            {
                newExpression.Left = ResolveComparisonFilterExpression(expression.Left as BinaryExpression);
            }
            else
            {
                newExpression.Left = ResolveBinaryFilterExpression(expression.Left as BinaryExpression, newExpression.Left);
            }
            
            if (expression.Right.GetType().Name == MethodBinaryExpression)
            {
                newExpression.Right = ResolveComparisonFilterExpression(expression.Right as BinaryExpression);
            }
            else
            {
                newExpression.Right = ResolveBinaryFilterExpression(expression.Right as BinaryExpression, newExpression.Right);
            }
            
            return newExpression;
        }

        // method to resolve final comparison (==, !=, >, etc.) expressions in the filter expression
        private BlBinaryExpression ResolveComparisonFilterExpression(BinaryExpression expression)
        {
            if (expression.Left.GetType().Name != PropertyExpressionType)
            {
                throw new IncorrectFilterArgumentStructureError($"Left operand of the filter expression has to be a property accessor . You provided {expression.Left.GetType().Name}");
            }
            
            BlBinaryExpression resultExpression = new BlBinaryExpression();

            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                {
                    resultExpression.Operator = BlOperator.Eq;
                    break;
                }
                case ExpressionType.NotEqual:
                {
                    resultExpression.Operator = BlOperator.NotEq;
                    break;
                }
                case ExpressionType.GreaterThan:
                {
                    resultExpression.Operator = BlOperator.Grt;
                    break;
                }
                case ExpressionType.GreaterThanOrEqual:
                {
                    resultExpression.Operator = BlOperator.GrtOrEq;
                    break;
                }
                case ExpressionType.LessThan:
                {
                    resultExpression.Operator = BlOperator.Ls;
                    break;
                }
                case ExpressionType.LessThanOrEqual:
                {
                    resultExpression.Operator = BlOperator.LsOrEq;
                    break;
                }
                default:
                {
                    //todo: replace with custom exception
                    throw new NotSupportedException("incorrect binary operator");
                }
            }

            if (expression.Left is MemberExpression propAccessor)
            {
                resultExpression.PropName = propAccessor.Member.Name;
            }
            resultExpression.Value = Expression.Lambda(expression.Right).Compile().DynamicInvoke();
            resultExpression.IsLeaf = true;

            return resultExpression;
        }

        private string ResolveSortExpression<TPawn>(Expression<Func<TPawn, object>> sortProperty) where TPawn : BlsPawn, new()
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        struct Connection
        {
            public BlsPawn From { get; set; }
            public BlsPawn To { get; set; }
            public string RelationName { get; set; }
        }
    }
}