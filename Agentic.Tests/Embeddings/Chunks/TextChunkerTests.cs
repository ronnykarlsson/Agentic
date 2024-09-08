using Agentic.Embeddings.Chunks;
using System.Text;

namespace Agentic.Tests.Embeddings.Chunks
{
    [TestFixture]
    public class TextChunkerTests
    {
        private const int _defaultChunkSize = 512;

        [Test]
        public void EmptyInput_ReturnsEmptyList()
        {
            var result = TextChunker.ChunkText("");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ShortInput_ReturnsSingleChunk()
        {
            var input = "Short text";
            var result = TextChunker.ChunkText(input, 100);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Chunk, Is.EqualTo(input));
        }

        [Test]
        public void InputExactlyAtChunkSize_ReturnsSingleChunk()
        {
            var input = new string('a', _defaultChunkSize);
            var result = TextChunker.ChunkText(input);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Chunk.Length, Is.EqualTo(_defaultChunkSize));
        }

        [Test]
        public void InputSlightlyOverChunkSize_ReturnsOneChunk()
        {
            var input = new string('a', _defaultChunkSize) + "extra";
            var result = TextChunker.ChunkText(input);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void LongInputWithoutDelimiters_SplitsCorrectly()
        {
            var input = new string('a', _defaultChunkSize * 3);
            var result = TextChunker.ChunkText(input);
            Assert.That(result.Count, Is.GreaterThan(1));
            Assert.That(result.All(chunk => chunk.Chunk.Length <= _defaultChunkSize * 1.3));
        }

        [Test]
        public void InputWithOnlyCommas_SplitsCorrectly()
        {
            var input = "word,word,word,word,word,word,";
            var result = TextChunker.ChunkText(input, 10);
            Assert.That(result.Count, Is.GreaterThan(1));
            Assert.That(result.All(chunk => chunk.Chunk.EndsWith(",")));
        }

        [Test]
        public void InputWithMixedDelimiters_SplitsCorrectly()
        {
            var input = "Sentence one. Sentence two, with a comma. Sentence three! Sentence four?";
            var result = TextChunker.ChunkText(input, 30);
            Assert.That(result.Count, Is.GreaterThan(1));
            Assert.That(result.All(chunk =>
                chunk.Chunk.TrimEnd().EndsWith(".") ||
                chunk.Chunk.TrimEnd().EndsWith("!") ||
                chunk.Chunk.TrimEnd().EndsWith("?")));
        }

        [Test]
        public void InputWithDelimitersAtStartAndEnd_HandlesCorrectly()
        {
            var input = "\n\nText starts here. Text ends here.\n\n";
            var result = TextChunker.ChunkText(input, 20);
            Assert.That(result.First().Chunk.StartsWith("\n\n"));
            Assert.That(result.Last().Chunk.EndsWith("\n\n"));
        }

        [Test]
        public void VeryLargeInput_HandlesEfficiently()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                sb.AppendLine($"This is line {i} of the very large input text.");
            }
            var input = sb.ToString();

            var result = TextChunker.ChunkText(input);
            Assert.That(result.Count, Is.GreaterThan(100));
            Assert.That(result.All(chunk => chunk.Chunk.Length <= _defaultChunkSize * 1.3));
        }

        [Test]
        public void InputWithRepeatedDelimiters_HandlesCorrectly()
        {
            var input = "Word.\n\n\n\nNext paragraph.\n\nLast paragraph.";
            var result = TextChunker.ChunkText(input, 17);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Chunk.EndsWith("\n\n\n\n"));
            Assert.That(result[1].Chunk.EndsWith("\n\n"));
        }

        [Test]
        public void InputWithUnicodeAndEmoji_PreservesCharacters()
        {
            var input = "Unicode: áéíóú. Emoji: 😀🌍🌈. More text here.";
            var result = TextChunker.ChunkText(input, 20);
            var reconstructed = string.Join("", result.Select(chunk => chunk.Chunk));
            Assert.That(reconstructed, Is.EqualTo(input));
        }

        [Test]
        public void InputWithDifferentLineBreaks_HandlesCorrectly()
        {
            var input = "Line1\nLine2\rLine3\r\nLine4";
            var result = TextChunker.ChunkText(input, 7);
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result[0].Chunk, Is.EqualTo("Line1\n"));
            Assert.That(result[1].Chunk, Is.EqualTo("Line2\r"));
            Assert.That(result[2].Chunk, Is.EqualTo("Line3\r\n"));
            Assert.That(result[3].Chunk, Is.EqualTo("Line4"));
        }

        [Test]
        public void InputWithVeryLongWord_SplitsCorrectly()
        {
            var longWord = new string('a', _defaultChunkSize * 2);
            var input = $"Short text. {longWord}. More short text.";
            var result = TextChunker.ChunkText(input);
            Assert.That(result.Count, Is.GreaterThan(2));
            Assert.That(result.All(chunk => chunk.Chunk.Length <= _defaultChunkSize * 1.3));
        }

        [Test]
        public void InputWithVaryingWhitespace_PreservesWhitespace()
        {
            var input = "Word   word     word\n\n\nword";
            var result = TextChunker.ChunkText(input, 10);
            var reconstructed = string.Join("", result.Select(chunk => chunk.Chunk));
            Assert.That(reconstructed, Is.EqualTo(input));
        }

        [Test]
        public void EnsureAllTextIncluded()
        {
            var input = "This is a long input text that should be split into multiple chunks while ensuring all text is included.";
            var result = TextChunker.ChunkText(input, 20);
            var reconstructed = string.Join("", result.Select(chunk => chunk.Chunk));
            Assert.That(reconstructed, Is.EqualTo(input));
        }

        [Test]
        public void EnsureNoOverlappingText()
        {
            var input = "This is a test input with enough length to be split into multiple chunks.";
            var result = TextChunker.ChunkText(input, 20);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.That(result[i + 1].TextStart, Is.GreaterThan(result[i].TextEnd));
            }
        }

        [Test]
        public void SmallChunksAreMergedTogether()
        {
            var input = "This is a long input text that should be split into multiple chunks and merged together.";
            var result = TextChunker.ChunkText(input, 50);            
            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}
