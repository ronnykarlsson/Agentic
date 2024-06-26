using System.Collections.Generic;
using System.Text.Json;

namespace Agentic.Helpers
{
    internal static class JsonHelpers
    {
        public static List<JsonElement> ExtractJsonElements(string input)
        {
            var validJsonElements = new List<JsonElement>();
            int braceCount = 0;
            int start = -1;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (c == '"')
                    {
                        inString = false;
                    }
                    continue;
                }

                if (c == '{')
                {
                    if (braceCount == 0)
                    {
                        start = i;
                    }
                    braceCount++;
                }
                else if (c == '}')
                {
                    braceCount--;
                    if (braceCount == 0 && start != -1)
                    {
                        TryAddJsonObject(input, start, i, validJsonElements);
                        start = -1;
                    }
                }
                else if (c == '"')
                {
                    inString = true;
                }
                else if (char.IsWhiteSpace(c))
                {
                    continue;
                }
            }

            // Check if there's an unclosed object that spans the entire input
            if (braceCount == 1 && start == 0)
            {
                TryAddJsonObject(input, start, input.Length - 1, validJsonElements);
            }

            return validJsonElements;
        }

        private static void TryAddJsonObject(string input, int start, int end, List<JsonElement> validJsonElements)
        {
            string jsonCandidate = input.Substring(start, end - start + 1);
            try
            {
                var jsonElement = JsonDocument.Parse(jsonCandidate).RootElement;
                if (jsonElement.ValueKind == JsonValueKind.Object)
                {
                    validJsonElements.Add(jsonElement);
                }
            }
            catch (JsonException)
            {
                // Ignore invalid JSON
            }
        }
    }
}
