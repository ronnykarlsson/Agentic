using System.Text.RegularExpressions;

namespace AutoSharp.Utilities
{
    public static class TokenHelpers
    {
        private static readonly Regex _tokenRegex = new Regex(@"\p{L}+('\p{L}+)*|[\p{N}\p{P}\p{S}]+", RegexOptions.Compiled);
        public static int EstimateTokenCount(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return _tokenRegex.Matches(text).Count;
        }
    }
}
