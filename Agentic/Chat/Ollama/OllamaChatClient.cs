﻿using Agentic.Clients.Ollama;
using Agentic.Exceptions;
using Agentic.Tools;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Agentic.Chat.Ollama
{
    public class OllamaChatClient : ChatClient<OllamaRequest>, IOllamaChatClient
    {
        private readonly IOllamaClient _client;
        private readonly string _model;

        public OllamaChatClient(IOllamaClient client, IConfiguration configuration)
            : base(int.Parse(configuration["Ollama:MaxTokens"] ?? "4096"))
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _client = new OllamaClient(configuration, null);
            _model = configuration["Ollama:Model"] ?? "orca2";
        }

        public OllamaChatClient(IOllamaClient client, string model, int maxTokens, Toolbox toolbox)
            : base(maxTokens)
        {
            _client = client;
            _model = model;
            Toolbox = toolbox;
        }

        public override async Task<ChatMessage> SendRequestAsync(OllamaRequest request)
        {
            OllamaResponse response;
            try
            {
                response = await _client.SendRequestAsync(request).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                throw new ChatException("Failed to send request", e);
            }

            var responseMessage = response.Message.Content;
            var chatMessage = new ChatMessage(Role.Assistant, responseMessage);
            return chatMessage;
        }

        protected override OllamaRequest CreateRequest()
        {
            var request = new OllamaRequest();
            request.Model = _model;
            return request;
        }

        protected override void AddRequestMessage(OllamaRequest request, ChatMessage message)
        {
            request.Messages.Add(new OllamaMessage { Role = message.Role.ToString(), Content = message.Content });
        }
    }
}
