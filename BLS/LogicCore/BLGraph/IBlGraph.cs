using System.Collections.Generic;

namespace BLS
{
    internal interface IBlGraph
    {
        void RegisterPawns(BlsPawn[] pawns);
        void CompileGraph();
        List<BlGraphContainer> CompiledCollections { get; }
        List<BlGraphRelation> CompiledRelations { get; }
        void VerifyUniqueNames(BlsPawn[] pawns);
    }
}