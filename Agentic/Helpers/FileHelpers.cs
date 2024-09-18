using System;
using System.IO;

namespace Agentic.Helpers
{
    internal static class FileHelpers
    {
        public static string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            if (path.StartsWith("~"))
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                path = Path.Combine(homeDirectory, path.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            }

            return Path.GetFullPath(path);
        }
    }
}
