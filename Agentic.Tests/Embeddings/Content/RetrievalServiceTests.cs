using Agentic.Embeddings;
using Agentic.Embeddings.Content;
using Agentic.Embeddings.Context;
using Agentic.Embeddings.Store;
using Moq;

namespace Agentic.Tests.Embeddings.Content
{
    [TestFixture]
    public class RetrievalServiceTests
    {
        private Mock<IEmbeddingService> _mockEmbeddingService;
        private Mock<IEmbeddingStore> _mockEmbeddingStore;
        private Mock<IEmbeddingContext> _mockEmbeddingContext;
        private RetrievalService _retrievalService;
        private float[] _testEmbedding;

        [SetUp]
        public void Setup()
        {
            _testEmbedding = new float[] { 0.1f, 0.2f, 0.3f };

            _mockEmbeddingService = new Mock<IEmbeddingService>();
            _mockEmbeddingStore = new Mock<IEmbeddingStore>();
            _mockEmbeddingContext = new Mock<IEmbeddingContext>();

            _mockEmbeddingService.Setup(s => s.GetEmbedding(It.IsAny<string>())).Returns(_testEmbedding);
            _mockEmbeddingContext.SetupGet(c => c.Service).Returns(_mockEmbeddingService.Object);
            _mockEmbeddingContext.SetupGet(c => c.Store).Returns(_mockEmbeddingStore.Object);

            _retrievalService = new RetrievalService(_mockEmbeddingContext.Object);
        }

        [Test]
        public void RetrieveRelevantDocuments_ReturnsExpectedResults()
        {
            // Arrange
            var searchResults = new List<SearchResult>
            {
                new SearchResult("doc1:0", 0.9, "Content 1")
            };
            _mockEmbeddingStore.Setup(s => s.FindClosestDocuments(It.IsAny<float[]>(), It.IsAny<int>()))
                .Returns(searchResults);

            // Act
            var result = _retrievalService.RetrieveRelevantDocuments(new[] { "test query" }, 1).ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo("doc1:0"));
            Assert.That(result.First().Content, Is.EqualTo("Content 1"));
        }

        [Test]
        public void RetrieveRelevantDocuments_WithOptions_ReturnsExpectedResults()
        {
            // Arrange
            var searchResults = new List<SearchResult>
            {
                new SearchResult("doc1:1", 0.9, "Content 2")
            };

            var precedingDoc = new Document("doc1:0", "Content 1", _testEmbedding, new Dictionary<string, string>());
            var followingDoc = new Document("doc1:2", "Content 3", _testEmbedding, new Dictionary<string, string>());

            _mockEmbeddingStore.Setup(s => s.FindClosestDocuments(It.IsAny<float[]>(), It.IsAny<int>()))
                .Returns(searchResults);
            _mockEmbeddingStore.Setup(s => s.GetDocumentById("doc1:0"))
                .Returns(precedingDoc);
            _mockEmbeddingStore.Setup(s => s.GetDocumentById("doc1:2"))
                .Returns(followingDoc);

            // Act
            var options = new RetrievalOptions { PrecedingChunks = 1, FollowingChunks = 1 };
            var result = _retrievalService.RetrieveRelevantDocuments(new[] { "test query" }, 1, options).ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));

            // The result should contain all three documents
            Assert.That(result.Any(r => r.Id == "doc1:0"), Is.True);
            Assert.That(result.Any(r => r.Id == "doc1:1"), Is.True);
            Assert.That(result.Any(r => r.Id == "doc1:2"), Is.True);
        }

        [Test]
        public void RetrieveRelevantDocuments_IncludesMetadataInResults()
        {
            // Arrange
            var metadata = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
            var document = new Document("doc1:0", "Content 1", _testEmbedding, metadata);

            _mockEmbeddingStore.Setup(s => s.FindClosestDocuments(It.IsAny<float[]>(), It.IsAny<int>()))
                .Returns(new List<SearchResult> { new SearchResult(document.Id, 0.9, document.Content, document.Metadata) });

            // Act
            var result = _retrievalService.RetrieveRelevantDocuments(new[] { "test query" }, 1).ToList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Metadata, Is.Not.Null);
            Assert.That(result[0].Metadata, Contains.Key("key1").WithValue("value1"));
            Assert.That(result[0].Metadata, Contains.Key("key2").WithValue("value2"));
        }
    }
}