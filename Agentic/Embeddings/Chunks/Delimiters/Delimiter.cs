using System;
using System.Linq;

namespace Agentic.Embeddings.Chunks.Delimiters
{
    public abstract class Delimiter
    {
        public virtual bool TryFindNext(string text, int startIndex, out int delimiterStart, out int delimiterEnd)
        {
            delimiterStart = -1;
            delimiterEnd = -1;

            for (int i = startIndex; i < text.Length; i++)
            {
                if (IsMatch(text, i))
                {
                    delimiterStart = i;
                    delimiterEnd = GetEndIndex(text, i);
                    return true;
                }
            }

            return false;
        }

        public abstract bool IsMatch(string text, int index);

        protected char[] DelimiterCharacters = new char[]
        {
            '\r', '\n', ' ', '\t',
            '.', ',', '!', '?'
        };

        protected virtual int GetEndIndex(string text, int startIndex)
        {
            int i = startIndex + 1;
            while (i < text.Length && DelimiterCharacters.Contains(text[i]))
            {
                i++;
            }

            return i;
        }

        protected char? GetChar(string text, int index)
        {
            if (index < text.Length) return text[index];
            return null;
        }
    }
}
