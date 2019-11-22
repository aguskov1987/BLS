using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ChangeTracking;

[assembly: InternalsVisibleTo("BLS.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace BLS
{
    public class Bls
    {
        private IBlGraph _graph;
        private IBlStorageProvider _storageProvider;
        private List<BlsPawn> _toAdd = new List<BlsPawn>();
        private List<int> _toConnect = new List<int>();
        private List<int> _toDisconnect = new List<int>();
        private List<BlsPawn> _toRemove = new List<BlsPawn>();

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
        /// <see cref="Bls.StoreChanges()"/> method
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
        /// <typeparam name="TPawn">Type of the pawn</typeparam>
        /// <returns>Storage Cursor containing the resulting collection of pawns</returns>
        /// <exception cref="NotImplementedException"></exception>
        public StorageCursor<TPawn> Find<TPawn>(
            bool includeSoftDeleted = false,
            Expression<Func<TPawn, bool>> filter = null) where TPawn: BlsPawn
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call the method to search for pawns based on the search term, optionally specifiying
        /// any additional filters
        /// </summary>
        /// <param name="searchTerm">The term to search</param>
        /// <param name="additionalFilters">Boolean expression specifying any additional filter conditions</param>
        /// <param name="includeSoftDeleted">Also retrieve soft deleted pawns if set to true; false by default</param>
        /// <param name="searchProperties">List of property expressions to apply the search on. Only properties
        /// marked with the <see cref="BLS.Functional.FullTextSearchable"/> attribute are allowed in the list of</param>
        /// <typeparam name="TPawn">Type of the pawn</typeparam>
        /// <returns>Storage Cursor containing the resulting collection of pawns</returns>
        /// <exception cref="NotImplementedException"></exception>
        public StorageCursor<TPawn> Search<TPawn>(
            string searchTerm,
            Expression<Func<TPawn, bool>> additionalFilters = null,
            bool includeSoftDeleted = false,
            params Expression<Func<TPawn, string>>[] searchProperties) where TPawn: BlsPawn
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
        public T GetById<T>(string id)
        {
            throw new NotImplementedException();
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
        /// <see cref="Bls.PersistChanges()"/> method.
        /// </remarks>
        public void Delete<TPawn>(TPawn pawn) where TPawn: BlsPawn
        {
            if (_toAdd.Contains(pawn))
            {
                _toAdd.Remove(pawn);
                //todo: remove any in-memory connections
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
    }
}