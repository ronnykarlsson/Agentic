using Agentic.Agents;
using Agentic.Chat;
using Agentic.Embeddings.Content;
using Agentic.Embeddings.Store;
using Agentic.Workspaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agentic.Tests.Workspaces
{
    [TestFixture]
    public class RagWorkspaceTests
    {
        private Mock<IContentProcessor> _mockContentProcessor;
        private Mock<IRetrievalService> _mockRetrievalService;
        private RagWorkspace _ragWorkspace;
        private AgentExecutionContext _executionContext;

        [SetUp]
        public void Setup()
        {
            _mockContentProcessor = new Mock<IContentProcessor>();
            _mockRetrievalService = new Mock<IRetrievalService>();
            _ragWorkspace = new RagWorkspace(_mockContentProcessor.Object, _mockRetrievalService.Object);

            // Create a minimal execution context with a few messages
            var message1 = new ChatMessage(Role.User, "First message");
            var message2 = new ChatMessage(Role.Assistant, "Response");
            var message3 = new ChatMessage(Role.User, "Query about content");

            var node1 = new ChatMessageNode(message1);
            var node2 = new ChatMessageNode(message2);
            node2.Previous = node1;
            var node3 = new ChatMessageNode(message3);
            node3.Previous = node2;

            var chatMessages = new ChatMessageLinkedList();
            chatMessages.AddLast(message1);
            chatMessages.AddLast(message2);
            chatMessages.AddLast(message3);

            _executionContext = new AgentExecutionContext
            {
                Messages = chatMessages
            };
        }

        [Test]
        public void Initialize_WithDefaultValues_SetsCorrectDefaults()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" }
            };

            // Act
            _ragWorkspace.Initialize(parameters);

            // Assert
            _mockContentProcessor.Verify(cp => cp.ProcessFiles(It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public void Initialize_WithSurroundingChunksParameters_SetsCorrectValues()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" },
                { "precedingChunks", "2" },
                { "followingChunks", "3" }
            };

            // Act
            _ragWorkspace.Initialize(parameters);

            // Assert
            _mockContentProcessor.Verify(cp => cp.ProcessFiles(It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public void GetPrompt_WithDefaultSurroundingChunks_CallsRetrieveWithCorrectParameters()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" }
            };
            _ragWorkspace.Initialize(parameters);

            var searchResults = new List<SearchResult>
            {
                new SearchResult("doc1:0", 0.9, "Content 1"),
                new SearchResult("doc1:1", 0.8, "Content 2")
            };

            _mockRetrievalService.Setup(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 1 && o.FollowingChunks == 1)))
                .Returns(searchResults);

            // Act
            var prompt = _ragWorkspace.GetPrompt(_executionContext);

            // Assert
            _mockRetrievalService.Verify(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 1 && o.FollowingChunks == 1)), Times.Once);
            
            // Check that the prompt contains the contents from the search results
            Assert.That(prompt, Does.Contain("Content 1"));
            Assert.That(prompt, Does.Contain("Content 2"));
        }

        [Test]
        public void GetPrompt_WithCustomSurroundingChunks_CallsRetrieveWithCorrectParameters()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" },
                { "precedingChunks", "2" },
                { "followingChunks", "3" }
            };
            _ragWorkspace.Initialize(parameters);

            var searchResults = new List<SearchResult>
            {
                new SearchResult("doc1:0", 0.9, "Content 1"),
                new SearchResult("doc1:1", 0.8, "Content 2")
            };

            _mockRetrievalService.Setup(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 2 && o.FollowingChunks == 3)))
                .Returns(searchResults);

            // Act
            var prompt = _ragWorkspace.GetPrompt(_executionContext);

            // Assert
            _mockRetrievalService.Verify(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 2 && o.FollowingChunks == 3)), Times.Once);
            
            // Check that the prompt contains the contents from the search results
            Assert.That(prompt, Does.Contain("Content 1"));
            Assert.That(prompt, Does.Contain("Content 2"));
        }

        [Test]
        public void GetPrompt_WithZeroSurroundingChunks_CallsRetrieveWithCorrectParameters()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" },
                { "precedingChunks", "0" },
                { "followingChunks", "0" }
            };
            _ragWorkspace.Initialize(parameters);

            var searchResults = new List<SearchResult>
            {
                new SearchResult("doc1:0", 0.9, "Content 1")
            };

            _mockRetrievalService.Setup(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 0 && o.FollowingChunks == 0)))
                .Returns(searchResults);

            // Act
            var prompt = _ragWorkspace.GetPrompt(_executionContext);

            // Assert
            _mockRetrievalService.Verify(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 0 && o.FollowingChunks == 0)), Times.Once);
        }

        [Test]
        public void Initialize_WithInvalidSurroundingChunksValues_ThrowsException()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" },
                { "precedingChunks", "invalid" }
            };

            // Act & Assert
            Assert.Throws<FormatException>(() => _ragWorkspace.Initialize(parameters));
        }

        [Test]
        public void GetPrompt_WithNoMessages_ReturnsNull()
        {
            // Arrange
            var emptyMessages = new ChatMessageLinkedList();
            var emptyContext = new AgentExecutionContext
            {
                Messages = emptyMessages
            };
            
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" }
            };
            _ragWorkspace.Initialize(parameters);

            // Act
            var prompt = _ragWorkspace.GetPrompt(emptyContext);

            // Assert
            Assert.That(prompt, Is.Null);
        }

        [Test]
        public void GetPrompt_WithNoRelevantDocuments_ReturnsNull()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" }
            };
            _ragWorkspace.Initialize(parameters);

            _mockRetrievalService.Setup(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 1 && o.FollowingChunks == 1)))
                .Returns(new List<SearchResult>());

            // Act
            var prompt = _ragWorkspace.GetPrompt(_executionContext);

            // Assert
            Assert.That(prompt, Is.Null);
        }

        [Test]
        public void GetPrompt_WithCustomHeader_UsesCorrectHeader()
        {
            // Arrange
            var parameters = new Dictionary<string, string>
            {
                { "file", "test.txt" },
                { "header", "Custom Header:" }
            };
            _ragWorkspace.Initialize(parameters);

            var searchResults = new List<SearchResult>
            {
                new SearchResult("doc1:0", 0.9, "Content 1")
            };

            _mockRetrievalService.Setup(rs => rs.RetrieveRelevantDocuments(
                It.IsAny<IEnumerable<string>>(), It.IsAny<int>(),
                It.Is<RetrievalOptions>(o => o.PrecedingChunks == 1 && o.FollowingChunks == 1)))
                .Returns(searchResults);

            // Act
            var prompt = _ragWorkspace.GetPrompt(_executionContext);

            // Assert
            Assert.That(prompt, Does.StartWith("Custom Header:"));
        }
    }
}