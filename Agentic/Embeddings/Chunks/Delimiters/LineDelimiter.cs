namespace Agentic.Embeddings.Chunks.Delimiters
{
    public class LineDelimiter : Delimiter
    {
        public override bool IsMatch(string text, int index)
        {
            var char1 = GetChar(text, index);
            if (char1 != '\n' && char1 != '\r') return false;

            var char2 = GetChar(text, index + 1);
            if (char2 != '\n' && char2 != '\r') return true;

            if (char1 == char2) return false;

            var char3 = GetChar(text, index + 2);
            if (char3 != '\n' && char3 != '\r') return true;

            return false;
        }
    }
}
