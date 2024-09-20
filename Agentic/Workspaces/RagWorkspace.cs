﻿using Agentic.Agents;
using Agentic.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Agentic.Chat;
using Agentic.Embeddings.Content;
using Agentic.Helpers;

namespace Agentic.Workspaces
{
    public class RagWorkspace : IWorkspace
    {
        private readonly IContentProcessor _contentProcessor;
        private readonly IRetrievalService _retrievalService;
        private readonly int _numberOfChatMessages = 3;
        private readonly int _retrieveDocumentCount = 3;

        public RagWorkspace(IContentProcessor contentProcessor, IRetrievalService retrievalService)
        {
            _contentProcessor = contentProcessor ?? throw new ArgumentNullException(nameof(contentProcessor));
            _retrievalService = retrievalService ?? throw new ArgumentNullException(nameof(retrievalService));
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.TryGetValue("chunkSize", out var chunkSizeString))
            {
                var chunkSize = int.Parse(chunkSizeString);
                _contentProcessor.SetChunkSize(chunkSize);
            }

            if (parameters.TryGetValue("file", out var fileValue))
            {
                _contentProcessor.ProcessFiles(new[] { FileHelpers.ResolvePath(fileValue) });
            }
            else if (parameters.TryGetValue("folder", out var folderValue))
            {
                _contentProcessor.ProcessFolders(new[] { FileHelpers.ResolvePath(folderValue) });
            }
            else
            {
                throw new ArgumentException("Parameters must include either 'file' or 'folder'.");
            }
        }

        public string GetPrompt(ExecutionContext context)
        {
            if (context?.Messages == null)
                throw new ArgumentNullException(nameof(context), "ChatContext or Messages cannot be null.");

            var lastMessages = GetLastNMessages(context.Messages.Tail, _numberOfChatMessages);

            if (lastMessages.Count == 0) return null;

            var messageContents = lastMessages.Select(msg => msg.Content);
            var relevantDocuments = _retrievalService.RetrieveRelevantDocuments(messageContents, _retrieveDocumentCount);

            if (!relevantDocuments.Any()) return null;

            string retrievedInfo = string.Join($"{Environment.NewLine}{Environment.NewLine}", relevantDocuments.Select(sr => sr.Content));
            string prompt = $"Information to use if relevant:{Environment.NewLine}{Environment.NewLine}{retrievedInfo}";
            return prompt;
        }

        public ITool[] GetWorkspaceTools()
        {
            return null;
        }

        private List<ChatMessage> GetLastNMessages(ChatMessageNode tail, int n)
        {
            var messages = new List<ChatMessage>();
            var current = tail;
            while (current != null && messages.Count < n)
            {
                messages.Add(current.Data);
                current = current.Previous;
            }
            messages.Reverse();
            return messages;
        }
    }
}