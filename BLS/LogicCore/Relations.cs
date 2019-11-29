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

    public sealed class RelatesToMany<T> : Relation<T> where T : BlsPawn, new()
    {
        public RelatesToMany(BlsPawn source, string multiplexer = "", int min = 0, int max = int.MaxValue)
            : base(source, multiplexer, min, max)
        {
        }
        
        public StorageCursor<T> Find(Expression<Func<T, bool>> filter = null, bool includeSoftDeleted = false, int batchSize = 200)
        {
            Connection[] connections = SourcePawn.SystemRef.ToConnect.Where(c => c.From == SourcePawn).ToArray();
            
            BlGraphContainer container =
                SourcePawn.SystemRef.Graph.CompiledCollections.FirstOrDefault(c => c.BlContainerName == typeof(T).Name);
            if (container == null)
            {
                throw new InvalidOperationException("collection not found");
            }
            //todo: add the flag to the in=memory/storage filters if including soft deleted pawns
            BlContainerProp softDeleteProp = container.Properties.FirstOrDefault(pr => pr.IsSoftDeleteProp);
            
            string id = SourcePawn.GetId();
            string relationName = SourcePawn.SystemRef.Graph.GetStorageRelationName(this);
            string containerName = SourcePawn.SystemRef.Graph.GetStorageContainerNameForPawn(SourcePawn);
            
            if (filter == null)
            {
                List<T> connectedPawns = connections.Select(c => (T) c.To).ToList();
                
                // if there is no id, the pawn has not been saved yet so there only return in-memory relations
                if (string.IsNullOrEmpty(id))
                {
                    return new StorageCursor<T>().AttachInMemPawns(connectedPawns);
                }
                
                // otherwise, also check the storage
                var cursorFromStorage =
                    SourcePawn.SystemRef.StorageProvider.GetByRelation<T>(id, relationName, containerName, null, batchSize);
                return cursorFromStorage.AttachInMemPawns(connectedPawns);
            }

            // the rest of the code assumes the filter is not null
            if (string.IsNullOrEmpty(id))
            {
                List<T> foundRelations = connections.Select(c => (T) c.To).ToList();
                var connectedPawns = foundRelations.Where(filter.Compile()).ToList();
                return new StorageCursor<T>().AttachInMemPawns(connectedPawns);
            }
            else
            {
                List<T> foundRelations = connections.Select(c => (T) c.To).ToList();
                List<T> connectedPawns = foundRelations.Where(filter.Compile()).ToList();
                BlBinaryExpression storageFilterExpression = SourcePawn.SystemRef.ResolveFilterExpression(filter);
                StorageCursor<T> cursorFromStorage =
                    SourcePawn.SystemRef.StorageProvider.GetByRelation<T>(id, relationName, containerName, storageFilterExpression, batchSize);
                return cursorFromStorage.AttachInMemPawns(connectedPawns);
            }
        }

        public int GetCount(bool includeSotDeleted = false)
        {
            throw new NotImplementedException();
        }
    }
}