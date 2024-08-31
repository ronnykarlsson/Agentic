using System.Diagnostics;

namespace Agentic.Embeddings.Chunks
{
    [DebuggerDisplay("{TextStart}-{TextEnd}: {Chunk}")]
    public class TextChunk
    {
        public int TextStart { get; set; }
        public int TextEnd { get; set; }
        public string Chunk { get; set; }
    }
}
