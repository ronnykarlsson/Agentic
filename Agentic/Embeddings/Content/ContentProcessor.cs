using Agentic.Embeddings.Chunks;
using Agentic.Embeddings.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Agentic.Embeddings.Context;

namespace Agentic.Embeddings.Content
{
    public class ContentProcessor : IContentProcessor
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IEmbeddingStore _embeddingStore;
        private int _chunkSize = 512;

        public ContentProcessor(IEmbeddingContext embeddingContext)
        {
            _embeddingService = embeddingContext?.Service ?? throw new ArgumentNullException(nameof(embeddingContext.Service));
            _embeddingStore = embeddingContext?.Store ?? throw new ArgumentNullException(nameof(embeddingContext.Store));
        }

        public void SetChunkSize(int chunkSize)
        {
            _chunkSize = chunkSize;
        }

        public void ProcessFiles(IEnumerable<string> filePaths)
        {
            if (filePaths == null) throw new ArgumentNullException(nameof(filePaths));

            foreach (var filePath in filePaths)
            {
                ProcessFile(filePath);
            }
        }

        public void ProcessFolders(IEnumerable<string> folderPaths)
        {
            if (folderPaths == null) throw new ArgumentNullException(nameof(folderPaths));

            var allFilePaths = folderPaths
                .Where(Directory.Exists)
                .SelectMany(folder => Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                .Distinct();

            ProcessFiles(allFilePaths);
        }

        private void ProcessFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    AddContentToStore(content, Path.GetFileName(filePath));
                }
                else
                {
                    Trace.TraceError($"File not found: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error processing file '{filePath}': {ex.Message}");
            }
        }

        private void AddContentToStore(string content, string source)
        {
            var chunks = TextChunker.ChunkText(content, _chunkSize);

            foreach (var chunk in chunks)
            {
                try
                {
                    float[] embedding = _embeddingService.GetEmbedding(chunk.Chunk);
                    var metadata = new Dictionary<string, string> { { "source", source } };
                    var document = new Document(Guid.NewGuid().ToString(), chunk.Chunk, embedding, metadata);
                    _embeddingStore.AddDocument(document);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error generating embedding for chunk from '{source}': {ex.Message}");
                }
            }
        }
    }
}
