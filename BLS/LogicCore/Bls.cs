﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BLS.Syncing;
using ChangeTracking;
// ReSharper disable PossibleUnintendedReferenceComparison

// ReSharper disable InvalidXmlDocComment

[assembly: InternalsVisibleTo("BLS.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace BLS
{
    public class Bls
    {
        internal readonly IBlStorageProvider StorageProvider;

        // new pawns to save to storage
        internal readonly HashSet<BlsPawn> ToAddBuffer = new HashSet<BlsPawn>();

        // changes in relations
        internal readonly HashSet<Connection> ToConnect = new HashSet<Connection>();
        internal readonly HashSet<Connection> ToDisconnect = new HashSet<Connection>();

        // pawns to remove from storage
        internal readonly HashSet<BlsPawn> ToRemove = new HashSet<BlsPawn>();

        // pawns retrieved from storage and assumed to be updated
        internal readonly HashSet<BlsPawn> ToUpdate = new HashSet<BlsPawn>();
        internal IBlGraph Graph;

        // dependency on internal Expression implementation as these types are internal and are subject to changes
        private string LogicalBinaryExpression = "LogicalBinaryExpression";

        // dependency on internal Expression implementation as these types are internal and are subject to changes
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
            StorageProvider = storageProvider;
            Graph = new BlGraph();
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
            StorageProvider = storageProvider;
            Graph = graph;
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
            if (Graph == null)
            {
                Graph = new BlGraph();
            }

            Graph.RegisterPawns(pawns);
            Graph.CompileGraph();
        }

        /// <summary>
        /// Call this method to sync compiled BL graph with the storage.
        /// </summary>
        /// <param name="validationOnly">If set to true (default to false), the storage engine
        /// will not make any persistent storage change. Only <see cref="SyncPlan"/> will be generated</param>
        /// <exception cref="InvalidOperationException"></exception>
        public SyncPlan SyncWithStorage(bool validationOnly = false)
        {
            if (Graph.CompiledCollections.Count > 0)
            {
                StorageProvider.Sync(Graph.CompiledCollections, Graph.CompiledRelations);
            }

            throw new InvalidOperationException("there are no pawns in the graph");
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
            if (Graph == null)
            {
                throw new BlGraphNotRegisteredError("BlGraph is not initialized");
            }

            if (Graph != null && (Graph.CompiledCollections == null || Graph.CompiledCollections.Count < 1))
            {
                throw new PawnNotRegisteredError(typeof(TPawn).Name);
            }

            if (Graph != null)
            {
                var registeredPawns = Graph.CompiledCollections.Select(c => c.BlContainerName).ToArray();
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
            ToAddBuffer.Add(traceablePawn);
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
            Expression<Func<TPawn, bool>> filter = null,
            Expression<Func<TPawn, IComparable>> sortProperty = null,
            Sort sortDir = Sort.Asc,
            bool includeSoftDeleted = false,
            int batchSize = 200) where TPawn : BlsPawn, new()
        {
            string container = Graph.GetStorageContainerNameForPawn(new TPawn());
            BlBinaryExpression filterExpression = ResolveFilterExpression(filter);
            filterExpression = ApplySoftDeleteFilterIfApplicable(includeSoftDeleted, filterExpression, new TPawn());
            string sortProp = ResolveSortExpression(sortProperty);

            var cursor = StorageProvider.FindInContainer<TPawn>(container, filterExpression, sortProp, sortDir.ToString(), batchSize);

            var additions = ToAddBuffer
                .Where(p => p.GetType() == typeof(TPawn))
                .Cast<TPawn>();

            var updates = ToUpdate
                .Where(p => p.GetType() == typeof(TPawn))
                .Select(p => (TPawn) p);

            
            if (filter == null)
            {
                cursor.AttachInMemoryPawns(additions.ToList()).AttachInMemoryPawns(updates.ToList());
            }

            return cursor;
        }

        /// <summary>
        /// Call the method to search for pawns based on the search term, optionally specifying
        /// any additional filters and sort
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
            Expression<Func<TPawn, string[]>> searchProperties,
            Expression<Func<TPawn, bool>> filter = null,
            Expression<Func<TPawn, IComparable>> sortProperty = null,
            Sort sortDir = Sort.Asc,
            bool includeSoftDeleted = false,
            int batchSize = 200) where TPawn : BlsPawn, new()
        {
            var container = Graph.GetStorageContainerNameForPawn(new TPawn());
            BlBinaryExpression filterExpression = ResolveFilterExpression(filter);
            filterExpression = ApplySoftDeleteFilterIfApplicable(includeSoftDeleted, filterExpression, new TPawn());
            string sortProp = ResolveSortExpression(sortProperty);

            return StorageProvider.SearchInContainer<TPawn>(
                container,
                ResolveSearchProperties(searchProperties),
                searchTerm,
                filterExpression,
                sortProp,
                sortDir.ToString(),
                batchSize);
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
            var container = Graph.GetStorageContainerNameForPawn(new T());
            if (Graph.CompiledCollections.Select(c => c.StorageContainerName).Any(c => c == container))
            {
                T result = StorageProvider.GetById<T>(id, container);
                result.SystemRef = this;
                var traceable = result.AsTrackable();
                ToUpdate.Add(traceable);
                return traceable;
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
        /// <returns>Cursor containing the result; only results from storage are returned</returns>
        public StorageCursor<T> LoadFromStorageByQuery<T>(string query) where T : new()
        {
            return StorageProvider.ExecuteQuery<T>(query);
        }

        /// <summary>
        /// Call this method to delete a pawn. Please keep in mind that all relations associated
        /// with this pawn will also be removed.
        /// </summary>
        /// <param name="pawn">Pawn to delete</param>
        /// <typeparam name="TPawn">Type of the pawn</typeparam>
        /// <remarks>Deletions will NOT be saved in storage unless you call the<see cref="PersistChanges()"/> method.
        /// </remarks>
        public void Delete<TPawn>(TPawn pawn) where TPawn : BlsPawn
        {
            ToRemove.Add(pawn);
            
            ToConnect.RemoveWhere(con => con.From == pawn || con.To == pawn);
            ToDisconnect.RemoveWhere(con => con.From == pawn || con.To == pawn);

            // case when the pawn is not saved yet
            if (pawn.GetId() == null)
            {
                //remove if it was added earlier
                ToAddBuffer.Remove(pawn);
            }
            else
            {
                // otherwise remove from the updates (in case it was added earlier)
                ToUpdate.Remove(pawn);
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
            if (StorageProvider == null)
            {
                throw new NoStorageProviderRegisteredError();
            }
            
            // 1. verify that all pawns meet the restrictions
            // 2. add new pawns        >>
            // 3. add new connections  >> in transaction
            // 4. remove connections   >>
            // 5. remove pawns         >>
        }

        /// <summary>
        /// Call this method to override the default naming encoder for container and relation names.
        /// Different storage providers may require different rules as how containers and relations are
        /// names. The default implementation uses CRC32 hashing to encode names of the containers and
        /// relation into consistent 8-byte strings. If you opt out to use your own naming encoder
        /// (for readability reasons, for example, as hashes are not back-readable), please keep in mind
        /// that encoded names of containers and relations must be unique, just like the names of pawns and
        /// and their relations have to be unique to correctly resolve the model.
        /// </summary>
        /// <param name="encoder">Implementation of the <see cref="IStorageNamingEncoder"/> interface</param>
        [ExcludeFromCodeCoverage] // pass through
        public void OverrideStorageNamingEncoder(IStorageNamingEncoder encoder)
        {
            Graph.OverrideStorageNamingEncoder(encoder);
        }
        
        internal void Connect(BlsPawn source, BlsPawn target, string relation)
        {
            if (ToRemove.Contains(source) || ToRemove.Contains(target))
            {
                throw new InvalidOperationException("you are trying to work with a pawn which has been deleted.");
            }
            
            var con = new Connection {From = source, To = target, RelationName = relation};
            ToConnect.Add(con);
            ToDisconnect.Remove(con);
        }
        
        internal void Disconnect(BlsPawn source, BlsPawn target, string relation)
        {
            if (ToRemove.Contains(source) || ToRemove.Contains(target))
            {
                throw new InvalidOperationException("you are trying to work with a pawn which has been deleted.");
            }
            
            var con = new Connection {From = source, To = target, RelationName = relation};
            if (con.From.GetId() != null && con.To.GetId() != null)
            {
                ToDisconnect.Add(con);
            }
            ToConnect.Remove(con);
        }

        internal BlBinaryExpression ResolveFilterExpression<TPawn>(Expression<Func<TPawn, bool>> filter)
            where TPawn : BlsPawn, new()
        {
            if (filter == null)
            {
                return null;
            }

            if (!(filter.Body is BinaryExpression expression))
            {
                throw new IncorrectFilterArgumentStructureError(
                    $"Filter expression has to be a binary expression. You provided {filter.Body.Type}");
            }

            if (expression.GetType().Name == MethodBinaryExpression)
            {
                return ResolveComparisonFilterExpression(expression);
            }

            if (expression.GetType().Name == LogicalBinaryExpression)
            {
                return ResolveBinaryFilterExpression(expression, new BlBinaryExpression());
            }

            throw new IncorrectFilterArgumentStructureError(
                $"Filter expression has to be a binary expression. You provided {filter.Body.Type}");
        }

        internal BlBinaryExpression ApplySoftDeleteFilterIfApplicable(bool includeDeleted, BlBinaryExpression filter, BlsPawn pawn)
        {
            string containerName = Graph.GetStorageContainerNameForPawn(pawn);
            BlGraphContainer container = Graph.CompiledCollections.FirstOrDefault(c => c.StorageContainerName == containerName);
            BlContainerProp softDeleteProp = container?.Properties.FirstOrDefault(prop => prop.IsSoftDeleteProp);

            if (softDeleteProp == null)
            {
                return filter;
            }
            
            var softDeleteClause = new BlBinaryExpression
            {
                PropName = softDeleteProp.Name, Operator = BlOperator.Eq, Value = includeDeleted
            };
            if (filter == null)
            {
                return softDeleteClause;
            }

            var newRoot = new BlBinaryExpression
            {
                Left = filter, Operator = BlOperator.And, Right = softDeleteClause
            };
            return newRoot;
        }

        internal string ResolveSortExpression<TPawn>(Expression<Func<TPawn, IComparable>> sortProperty)
            where TPawn : BlsPawn, new()
        {
            if (sortProperty == null)
            {
                string containerName = Graph.GetStorageContainerNameForPawn(new TPawn());
                BlGraphContainer container = Graph.CompiledCollections.FirstOrDefault(c => c.StorageContainerName == containerName);
                BlContainerProp defaultSortProperty = container?.Properties.FirstOrDefault(p => p.IsDefaultSort);

                return defaultSortProperty?.Name;
            }
            
            Expression expression = sortProperty.Body;
            if (expression is MemberExpression)
            {
                return expression.ToString().Split('.')[1];
            }

            throw new InvalidSortPropertyError($"Only property access expression are supported; you provided {expression.GetType().Name}");
        }

        // recursive method to resolve AND/OR binary filter expressions
        private BlBinaryExpression ResolveBinaryFilterExpression(BinaryExpression expression,
            BlBinaryExpression newExpression)
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
                newExpression.Left =
                    ResolveBinaryFilterExpression(expression.Left as BinaryExpression, newExpression.Left);
            }

            if (expression.Right.GetType().Name == MethodBinaryExpression)
            {
                newExpression.Right = ResolveComparisonFilterExpression(expression.Right as BinaryExpression);
            }
            else
            {
                newExpression.Right =
                    ResolveBinaryFilterExpression(expression.Right as BinaryExpression, newExpression.Right);
            }

            return newExpression;
        }

        // method to resolve final comparison (==, !=, >, etc.) expressions in the filter expression
        private BlBinaryExpression ResolveComparisonFilterExpression(BinaryExpression expression)
        {
            if (expression.Left.GetType().Name != PropertyExpressionType)
            {
                throw new IncorrectFilterArgumentStructureError(
                    $"Left operand of the filter expression has to be a property accessor . You provided {expression.Left.GetType().Name}");
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

        private List<string> ResolveSearchProperties<TPawn>(Expression<Func<TPawn,string[]>> searchProperties) where TPawn : BlsPawn, new()
        {
            NewArrayExpression newArray;
            
            try
            {
                newArray = (NewArrayExpression)searchProperties.Body;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException("Only array with property accessors are supported", ex);
            }
            
            var result = new List<string>();
            ReadOnlyCollection<Expression> items = newArray.Expressions;

            foreach (var member in items)
            {
                if (member is MemberExpression)
                {
                    string propName = member.ToString().Split('.')[1];
                    result.Add(propName);
                }
                else
                {
                    throw new InvalidOperationException("Only property access expression are supported in the array");
                }
            }

            return result;
        }
    }
}