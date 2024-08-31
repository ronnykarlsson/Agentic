using Agentic.Embeddings.Chunks.Delimiters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agentic.Embeddings.Chunks
{

    public class TextChunker
    {
        private const int _defaultChunkSize = 512;
        private static readonly Delimiter[] _delimiters = new Delimiter[]
        {
            new ParagraphDelimiter(),
            new SentenceDelimiter(),
            new LineDelimiter(),
            new ClauseDelimiter(),
            new WordDelimiter()
        };

        public static List<TextChunk> ChunkText(string input, int targetChunkSize = _defaultChunkSize)
        {
            if (string.IsNullOrEmpty(input))
                return new List<TextChunk>();

            if (targetChunkSize <= 0)
                throw new ArgumentException("Target chunk size must be positive", nameof(targetChunkSize));

            var textChunk = new TextChunk
            {
                Chunk = input,
                TextStart = 0,
                TextEnd = input.Length - 1
            };

            var chunks = new List<TextChunk> { textChunk };

            var chunksToSplit = new List<TextChunk>();
            if (input.Length > targetChunkSize) chunksToSplit.Add(textChunk);

            foreach (var delimiter in _delimiters)
            {
                if (!chunksToSplit.Any()) break;

                var newChunksToSplit = new List<TextChunk>();
                foreach (var largeChunk in chunksToSplit)
                {
                    var largeChunkIndex = chunks.IndexOf(largeChunk);
                    chunks.RemoveAt(largeChunkIndex);

                    var startIndex = largeChunk.TextStart;

                    // Split large chunk until end is reached
                    while (startIndex <= largeChunk.TextEnd)
                    {
                        var endIndex = FindNextChunkEnd(delimiter, largeChunk.Chunk, startIndex - largeChunk.TextStart);
                        if (endIndex == -1) endIndex = largeChunk.TextEnd + 1;
                        else endIndex += largeChunk.TextStart;
                        if (endIndex == startIndex) throw new InvalidOperationException("Delimiter returned 0 length");

                        var newChunk = new TextChunk
                        {
                            Chunk = largeChunk.Chunk.Substring(startIndex - largeChunk.TextStart, endIndex - startIndex),
                            TextStart = startIndex,
                            TextEnd = endIndex - 1
                        };

                        chunks.Insert(largeChunkIndex++, newChunk);
                        if (endIndex - startIndex > targetChunkSize) newChunksToSplit.Add(newChunk);

                        startIndex = endIndex;
                    }
                }

                chunksToSplit = newChunksToSplit;
            }

            if (chunksToSplit.Any())
            {
                // Divide chunks which couldn't be split with delimiters
                foreach (var largeChunk in chunksToSplit)
                {
                    var largeChunkIndex = chunks.IndexOf(largeChunk);
                    chunks.RemoveAt(largeChunkIndex);

                    var startIndex = largeChunk.TextStart;

                    var chunkSplitCount = largeChunk.Chunk.Length / targetChunkSize;
                    if (largeChunk.Chunk.Length % targetChunkSize > 0) chunkSplitCount++;

                    var chunkLength = largeChunk.Chunk.Length / chunkSplitCount;
                    if (largeChunk.Chunk.Length % chunkSplitCount > 0) chunkLength++;

                    for (int i = 0; i < chunkSplitCount; i++)
                    {
                        var endIndex = Math.Min(startIndex + chunkLength, largeChunk.TextEnd + 1);

                        var newChunk = new TextChunk
                        {
                            Chunk = largeChunk.Chunk.Substring(startIndex - largeChunk.TextStart, endIndex - startIndex),
                            TextStart = startIndex,
                            TextEnd = endIndex - 1
                        };

                        chunks.Insert(largeChunkIndex++, newChunk);

                        startIndex = endIndex;
                    }
                }
            }

            return chunks;
        }

        private static int FindNextChunkEnd(Delimiter delimiter, string input, int startIndex)
        {
            int maxEndIndex = Math.Min(startIndex, input.Length);
            int bestEndIndex = startIndex;

            if (delimiter.TryFindNext(input, startIndex, out int delimiterStart, out int delimiterEnd))
            {
                return delimiterEnd;
            }

            return -1;
        }
    }
}
