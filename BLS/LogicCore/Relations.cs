using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
// ReSharper disable MemberCanBePrivate.Global - need public visibility for reflection
// ReSharper disable UnusedAutoPropertyAccessor.Global - need getters for reflection

namespace BLS
{
    public abstract class Relation<T> where T : BlsPawn, new()
    {
        protected Relation(BlsPawn sourcePawn, string multiplexer, int minConnections, int maxConnections)
        {
            SourcePawn = sourcePawn;
            Multiplexer = multiplexer;
            MinConnections = minConnections;
            MaxConnections = maxConnections;
        }

        internal BlsPawn SourcePawn { get; }
        public string Multiplexer { get; }
        public int MinConnections { get; }
        public int MaxConnections { get; }
        
        public void Connect(T pawn)
        {
            var relationName = SourcePawn.SystemRef.Graph.GetStorageRelationName(this);
            SourcePawn.SystemRef.Connect(SourcePawn, pawn, relationName);
        }

        public void Disconnect(T pawn)
        {
            var relationName = SourcePawn.SystemRef.Graph.GetStorageRelationName(this);
            SourcePawn.SystemRef.Disconnect(SourcePawn, pawn, relationName);
        }
    }
    
    /// <summary>
    /// Use this class to create a one-to-one relation
    /// </summary>
    /// <typeparam name="T">Type of the pawn object</typeparam>
    public sealed class RelatesToOne<T> : Relation<T> where T : BlsPawn, new()
    {
        public RelatesToOne(BlsPawn source, string multiplexer = "") : base(source, multiplexer, 0, 1)
        {
        }

        public T Get(bool includeSoftDeleted = false)
        {
            return null;
        }
    }

    /// <summary>
    /// Use this type to create a one-to-many relation.
    /// </summary>
    /// <typeparam name="T">Type of the pawn object</typeparam>
    public sealed class RelatesToMany<T> : Relation<T> where T : BlsPawn, new()
    {
        /// <summary>
        /// Create a new one-to-many relation
        /// </summary>
        /// <param name="source">Source pawn</param>
        /// <param name="multiplexer">A string used to specify a unique relation in case more than one relation exists
        /// for one particular of pawn</param>
        /// <param name="min">Minimum allowed number of connected pawn objects; defaults to 0</param>
        /// <param name="max">Maximum allowed number of connected pawn objects; defaults to <see cref="int.MaxValue"/></param>
        public RelatesToMany(BlsPawn source, string multiplexer = "", int min = 0, int max = int.MaxValue)
            : base(source, multiplexer, min, max)
        {
        }

        /// <summary>
        /// Use this method to retrieve related objects of the certain <typeparam name="T"></typeparam> type. The method
        /// returns a cursor which contains pawns in storage as well as pawns currently sitting in the BLS memory
        /// </summary>
        /// <param name="filter">Optional filter to apply to the result set; the filter will be applied to both the im-memory
        /// and in-storage data</param>
        /// <param name="sortDir"></param>
        /// <param name="includeSoftDeleted">If set to true, the method ignores soft-delete flag if one is available on the
        /// pawn model. Set to false by default</param>
        /// <param name="batchSize">Controls the size of the batch of each incremental retrieval of pawns from storage. Defaults to 200 objects</param>
        /// <param name="sortProperty"></param>
        /// <returns>Cursor containing the result set</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public StorageCursor<T> Find(
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, IComparable>> sortProperty = null,
            Sort sortDir = Sort.Asc,
            bool includeSoftDeleted = false,
            int batchSize = 200)
        {
            Connection[] connections = SourcePawn.SystemRef.ToConnect
                .Where(c => c.From == SourcePawn)
                .ToArray();
            
            BlGraphContainer container = SourcePawn.SystemRef.Graph.CompiledCollections
                    .FirstOrDefault(c => c.BlContainerName == typeof(T).Name);
            
            if (container == null)
            {
                throw new InvalidOperationException("collection not found");
            }

            string id = SourcePawn.GetId();
            string relationName = SourcePawn.SystemRef.Graph.GetStorageRelationName(this);
            string containerName = SourcePawn.SystemRef.Graph.GetStorageContainerNameForPawn(SourcePawn);

            if (filter == null)
            {
                var filterExpression = SourcePawn.SystemRef
                    .ApplySoftDeleteFilterIfApplicable(includeSoftDeleted, null, new T());
                
                List<T> connectedPawns = connections.Select(c => (T) c.To).ToList();
                
                // if there is no id, the pawn has not been saved yet so there only return in-memory relations
                if (string.IsNullOrEmpty(id))
                {
                    return new StorageCursor<T>().AttachInMemoryPawns(connectedPawns).InjectBls(SourcePawn.SystemRef);
                }
                
                // otherwise, also check the storage

                string sort = SourcePawn.SystemRef.ResolveSortExpression(sortProperty);
                
                var cursorFromStorage =
                    SourcePawn.SystemRef.StorageProvider.GetByRelation<T>(id, relationName, containerName, filterExpression, sort, sortDir, batchSize);
                return cursorFromStorage.AttachInMemoryPawns(connectedPawns).InjectBls(SourcePawn.SystemRef);
            }

            // the rest of the code assumes the filter is not null
            if (string.IsNullOrEmpty(id))
            {
                List<T> foundRelations = connections.Select(c => (T) c.To).ToList();
                List<T> connectedPawns = foundRelations.Where(filter.Compile()).ToList();
                return new StorageCursor<T>().AttachInMemoryPawns(connectedPawns).InjectBls(SourcePawn.SystemRef);
            }
            else
            {
                List<T> foundRelations = connections.Select(c => (T) c.To).ToList();
                List<T> connectedPawns = foundRelations.Where(filter.Compile()).ToList();
                BlBinaryExpression storageFilterExpression = SourcePawn.SystemRef.ResolveFilterExpression(filter);
                
                storageFilterExpression = SourcePawn.SystemRef
                    .ApplySoftDeleteFilterIfApplicable(includeSoftDeleted, storageFilterExpression, new T());
                
                string sort = SourcePawn.SystemRef.ResolveSortExpression(sortProperty);
                
                StorageCursor<T> cursorFromStorage =
                    SourcePawn.SystemRef.StorageProvider.GetByRelation<T>(id, relationName, containerName, storageFilterExpression, sort, sortDir, batchSize);
                return cursorFromStorage.AttachInMemoryPawns(connectedPawns).InjectBls(SourcePawn.SystemRef);
            }
        }

        /// <summary>
        /// Call the method to get number of related pawns. The result will include both in-storage and in-memory pawns
        /// </summary>
        /// <param name="includeSotDeleted">If set to true, the method ignores soft-delete flag if one is available on the
        /// pawn model. Set to false by default</param>
        /// <returns>Number of related pawns</returns>
        /// <exception cref="NotImplementedException"></exception>
        public int GetCount(bool includeSotDeleted = false)
        {
            BlBinaryExpression filter = SourcePawn.SystemRef
                .ApplySoftDeleteFilterIfApplicable(includeSotDeleted, null, new T());

            string containerName = SourcePawn.SystemRef.Graph.GetStorageContainerNameForPawn(new T());
            
            return SourcePawn.SystemRef.StorageProvider.GetContainerCount(containerName, filter);
        }
    }
}