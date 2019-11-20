using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ChangeTracking;

namespace BLS
{
    public class Bls
    {
        private IBlStorageProvider _storageProvider;
        private BlGraph _graph;
        private List<BlsPawn> _toAdd;
        private List<BlsPawn> _toRemove;
        private List<int> _toConnect;
        private List<int> _toDisconnect;
        
        public Bls(IBlStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public void RegisterBlGraph(BlsPawn[] pawns)
        {
            _graph = new BlGraph(pawns);
            _graph.CompileGraph();
        }

        public TPawn SpawnNew<TPawn>() where TPawn : BlsPawn, new()
        {
            var registeredPawns = _graph.Pawns.Select(p => p.GetType().Name).ToArray();
            if (registeredPawns.All(p => p != typeof(TPawn).Name))
            {
                throw new PawnNotRegisteredError(typeof(TPawn).Name);
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

        public StorageCursor<T> Find<T>(Expression<Func<T, bool>> filter = null)
        {
            throw new NotImplementedException();
        }
        
        public StorageCursor<T> Search<T>(
            string searchTerm,
            Expression<Func<T, string[]>> searchProperties,
            Expression<Func<T, bool>> filter = null)
        {
            throw new NotImplementedException();
        }
        
        public T GetById<T>(string id)
        {
            throw new NotImplementedException();
        }
        
        public StorageCursor<T> GetByQuery<T>(string query) where T: new()
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T pawn)
        {
            throw new NotImplementedException();
        }
    }
}