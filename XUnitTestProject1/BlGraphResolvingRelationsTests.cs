using Xunit;

namespace BLS.Tests
{
    public class BlGraphResolvingRelationsTests
    {
        [Fact]
        public void should_resolve_law_firm_model_when_graph_is_compiled()
        {
            // Setup
            var graph = new BlGraph();
            graph.RegisterPawns(new BlsPawn[]
            {
                new LawFirm(), new Lawyer(), new Assistant(), new Matter(), new Client()
            });

            // Act
            graph.CompileGraph();

            // Assert
            Assert.NotEmpty(graph.CompiledCollections);
            Assert.NotEmpty(graph.CompiledRelations);
        }
    }
}