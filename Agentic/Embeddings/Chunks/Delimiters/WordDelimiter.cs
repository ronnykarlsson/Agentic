using System;
using System.Linq;

namespace Agentic.Embeddings.Chunks.Delimiters
{
    public class WordDelimiter : Delimiter
    {
        private static char[] _delimiters = new[] { ' ', '\t' };

        public override bool IsMatch(string text, int index)
        {
            return _delimiters.Contains(GetChar(text, index).GetValueOrDefault());
        }
    }
}
