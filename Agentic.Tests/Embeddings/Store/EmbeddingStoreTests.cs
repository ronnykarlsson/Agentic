using Agentic.Embeddings.Store;

namespace Agentic.Tests.Embeddings.Store
{
    [TestFixture]
    public class EmbeddingStoreTests
    {
        private EmbeddingStore _embeddingStore;
        private float[] _testEmbedding;

        [SetUp]
        public void Setup()
        {
            _embeddingStore = new EmbeddingStore();
            _testEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
        }

        [Test]
        public void GetDocumentById_WithExistingId_ReturnsDocument()
        {
            // Arrange
            var document = new Document("doc1:0", "Test Content", _testEmbedding, new Dictionary<string, string>());
            _embeddingStore.AddDocument(document);

            // Act
            var result = _embeddingStore.GetDocumentById("doc1:0");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("doc1:0"));
            Assert.That(result.Content, Is.EqualTo("Test Content"));
        }

        [Test]
        public void GetDocumentById_WithNonexistentId_ReturnsNull()
        {
            // Act
            var result = _embeddingStore.GetDocumentById("nonexistent");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDocumentById_WithMultipleDocuments_ReturnsCorrectOne()
        {
            // Arrange
            var document1 = new Document("doc1:0", "Content 1", _testEmbedding, new Dictionary<string, string>());
            var document2 = new Document("doc1:1", "Content 2", _testEmbedding, new Dictionary<string, string>());
            var document3 = new Document("doc1:2", "Content 3", _testEmbedding, new Dictionary<string, string>());
            
            _embeddingStore.AddDocument(document1);
            _embeddingStore.AddDocument(document2);
            _embeddingStore.AddDocument(document3);

            // Act
            var result = _embeddingStore.GetDocumentById("doc1:1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("doc1:1"));
            Assert.That(result.Content, Is.EqualTo("Content 2"));
        }

        [Test]
        public void GetDocumentById_WithEmptyStore_ReturnsNull()
        {
            // Act
            var result = _embeddingStore.GetDocumentById("any-id");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDocumentById_WithNullId_ReturnsNull()
        {
            // Act
            var result = _embeddingStore.GetDocumentById(null);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void AddDocument_WithMultipleDocumentsSameId_KeepsBoth()
        {
            // Arrange
            var document1 = new Document("doc1:0", "Content 1", _testEmbedding, new Dictionary<string, string>());
            var document2 = new Document("doc1:0", "Content 2", _testEmbedding, new Dictionary<string, string>());
            
            _embeddingStore.AddDocument(document1);
            _embeddingStore.AddDocument(document2);

            // Act
            var result = _embeddingStore.GetDocumentById("doc1:0");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("doc1:0"));
            Assert.That(result.Content, Is.EqualTo("Content 1"));
        }

        [Test]
        public void FindClosestDocuments_ReturnsDocumentsInOrderOfSimilarity()
        {
            // Arrange - Create documents with varying similarity (via different embeddings)
            var embedding1 = new float[] { 0.9f, 0.9f, 0.9f };  // Most similar to query
            var embedding2 = new float[] { 0.5f, 0.5f, 0.5f };  // Medium similarity
            var embedding3 = new float[] { 0.1f, 0.1f, 0.1f };  // Least similar

            var document1 = new Document("doc1:0", "Content 1", embedding1, new Dictionary<string, string>());
            var document2 = new Document("doc1:1", "Content 2", embedding2, new Dictionary<string, string>());
            var document3 = new Document("doc1:2", "Content 3", embedding3, new Dictionary<string, string>());
            
            _embeddingStore.AddDocument(document1);
            _embeddingStore.AddDocument(document2);
            _embeddingStore.AddDocument(document3);

            // Query embedding is [1,1,1], so embedding1 will be most similar
            var queryEmbedding = new float[] { 1.0f, 1.0f, 1.0f };

            // Act
            var results = _embeddingStore.FindClosestDocuments(queryEmbedding, 3);

            // Assert
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results[0].Id, Is.EqualTo("doc1:0")); // Most similar
            Assert.That(results[1].Id, Is.EqualTo("doc1:1")); // Medium similarity
            Assert.That(results[2].Id, Is.EqualTo("doc1:2")); // Least similar
        }

        [Test]
        public void FindClosestDocuments_WithLimitLessThanTotal_ReturnsOnlyRequestedNumber()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                var document = new Document($"doc1:{i}", $"Content {i}", _testEmbedding, new Dictionary<string, string>());
                _embeddingStore.AddDocument(document);
            }

            // Act
            var results = _embeddingStore.FindClosestDocuments(_testEmbedding, 3);

            // Assert
            Assert.That(results.Count, Is.EqualTo(3));
        }
    }
}