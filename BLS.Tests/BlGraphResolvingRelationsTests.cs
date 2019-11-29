using Xunit;

namespace BLS.Tests
{
    public class BlGraphResolvingRelationsTests
    {
        [Fact]
        public void should_fail_resolving_bl_graph_if_cannot_resolve_relations_1()
        {
            // Setup
            var graph = new BlGraph();
            graph.RegisterPawns(new BlsPawn[]{new InvalidParent1(), new InvalidChild1()});

            // Act & Assert
            Assert.Throws<DuplicateRelationInPawnError>(() => {graph.CompileGraph();});
        }
        
        [Fact]
        public void should_fail_resolving_bl_graph_if_cannot_resolve_relations_2()
        {
            // Setup
            var graph = new BlGraph();
            graph.RegisterPawns(new BlsPawn[]{new InvalidParent2(), new InvalidChild2()});

            // Act & Assert
            Assert.Throws<DuplicateRelationInPawnError>(() => {graph.CompileGraph();});
        }

        [Fact]
        public void should_resolve_law_firm_model_when_graph_is_compiled_1()
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
        
        [Fact]
        public void should_resolve_law_firm_model_when_graph_is_compiled_2()
        {
            // Setup
            var graph = new BlGraph();
            graph.RegisterPawns(new BlsPawn[]
            {
                new Car(), new Wheel()
            });

            // Act
            graph.CompileGraph();

            // Assert
            Assert.NotEmpty(graph.CompiledCollections);
            Assert.NotEmpty(graph.CompiledRelations);
        }
    }
}