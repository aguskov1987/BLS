using System;
using System.Collections.Generic;

namespace BLS
{
    public class BlRelationResolver
    {
        private List<BlEntity> _entities;

        public BlRelationResolver()
        {
            _entities = new List<BlEntity>();
        }

        public void AddEntityWithRelation(BlEntity entityType)
        {
            Type tp = entityType.GetType();
            _entities.Add(entityType);
        }

        public void VerifyRelationalIntegrity()
        {
            throw new NotImplementedException();
        }

        public List<string> GetResolvedContainers()
        {
            throw new NotImplementedException();
        }

        public List<Tuple<string, string, string>> GetResolvedRelations()
        {
            throw new NotImplementedException();
        }
    }
}