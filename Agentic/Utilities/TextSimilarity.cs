using System;

namespace Agentic.Utilities
{
    internal static class TextSimilarity
    {
        /// <summary>
        /// Determines if two texts are similar enough based on normalized Levenshtein distance.
        /// </summary>
        /// <param name="text1">The first text to compare.</param>
        /// <param name="text2">The second text to compare.</param>
        /// <param name="similarityThreshold">
        /// A value between 0 and 1. The closer to 1, the more similar the texts must be.
        /// For example, 0.9 means the texts must be at least 90% similar.
        /// </param>
        /// <returns>True if the texts are considered similar enough, otherwise false.</returns>
        public static bool Check(string text1, string text2, double similarityThreshold = 0.9)
        {
            if (text1 == text2) return true;
            if (text1 == null || text2 == null) return false;

            var distance = ComputeLevenshteinDistance(text1, text2);
            int maxLength = Math.Max(text1.Length, text2.Length);

            double similarity = 1.0 - ((double)distance / maxLength);
            return similarity >= similarityThreshold;
        }

        /// <summary>
        /// Computes the Levenshtein distance between two strings.
        /// </summary>
        private static int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            var d = new int[n + 1, m + 1];

            // Initialize the distance matrix
            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            // Compute the distance
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            return d[n, m];
        }
    }
}
