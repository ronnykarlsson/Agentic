using Agentic.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agentic.Tests.Chat
{
    public class ChatClientTests
    {
        public class CalculateTokensTests
        {
            [Test]
            public void CalculateTokensWithEmptyMessage()
            {
                var client = new TestChatClient();
                var message = new ChatMessage(Role.System, "");
                var tokens = client.GetTokenCount(message);
                Assert.That(tokens, Is.EqualTo(1));
            }

            [Test]
            public void CalculateTokensWithShortMessage()
            {
                var client = new TestChatClient();
                var message = new ChatMessage(Role.System, "Test");
                var tokens = client.GetTokenCount(message);
                Assert.That(tokens, Is.EqualTo(2));
            }

            [Test]
            public void CalculateTokensWithJsonMessage()
            {
                var client = new TestChatClient();
                var message = new ChatMessage(Role.System, "{\"Test\": 1}");
                var tokens = client.GetTokenCount(message);
                Assert.That(tokens, Is.EqualTo(7));
            }
        }
    }

    public class TestChatClient : ChatClient<TestChatClientRequest>
    {
        public TestChatClient() : base(100)
        {
        }

        public int GetTokenCount(ChatMessage message)
        {
            return CalculateTokens(message);
        }

        public override Task<ChatMessage> SendRequestAsync(TestChatClientRequest request)
        {
            return Task.FromResult(new ChatMessage(Role.Assistant, "Test"));
        }

        protected override void AddRequestMessage(TestChatClientRequest request, ChatMessage message)
        {
            request.Messages.Add(message);
        }

        protected override TestChatClientRequest CreateRequest()
        {
            return new TestChatClientRequest();
        }
    }

    public class TestChatClientRequest
    {
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
