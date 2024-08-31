using System;
using System.Linq;

namespace Agentic.Embeddings.Chunks.Delimiters
{
    public class SentenceDelimiter : Delimiter
    {
        private static char[] _delimiters = new[] { '.', '!', '?' };

        public override bool IsMatch(string text, int index)
        {
            return _delimiters.Contains(GetChar(text, index).GetValueOrDefault());
        }
    }
}
