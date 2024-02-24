using SharpToken;

namespace Agentic.Utilities
{
    public static class TokenHelpers
    {
        public static int CalculateTokenCount(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return GptEncoding.GetEncodingForModel("gpt-4").Encode(text).Count;
        }
    }
}
