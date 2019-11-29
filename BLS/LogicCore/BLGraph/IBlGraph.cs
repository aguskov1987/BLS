using System.Collections.Generic;

namespace BLS
{
    internal interface IBlGraph
    {
        void RegisterPawns(BlsPawn[] pawns);
        void CompileGraph();
        List<BlGraphContainer> CompiledCollections { get; }
        List<BlGraphRelation> CompiledRelations { get; }
        void OverrideStorageNamingEncoder(IStorageNamingEncoder encoder);
        string GetStorageContainerNameForPawn(BlsPawn pawn);
        string GetStorageRelationName<T>(Relation<T> relation) where T : BlsPawn, new();
    }
}