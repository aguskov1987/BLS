using System;
using System.Collections.Generic;

namespace BLS
{
    public interface IBlRelationResolver
    {
        void AddEntityWithRelation(BlEntity entityType);
        void VerifyRelationalIntegrity();
        List<string> GetResolvedContainers();
        List<Tuple<string, string, string>> GetResolvedRelations();
    }
}