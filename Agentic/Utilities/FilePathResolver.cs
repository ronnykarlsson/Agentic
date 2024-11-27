using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Agentic.Utilities
{
    /// <summary>
    /// Provides methods to resolve relative or absolute file paths to their full absolute paths.
    /// </summary>
    public static class FilePathResolver
    {
        /// <summary>
        /// Resolves a relative or absolute file path to its full absolute path.
        /// Attempts to locate the file or directory based on various base directories.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file or directory.</param>
        /// <returns>The full absolute path to the file or directory.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided path is null or empty.</exception>
        public static string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            // If the path is already absolute, return its full form
            if (Path.IsPathRooted(path))
            {
                return Path.GetFullPath(path);
            }

            // List of potential base directories
            var baseDirectories = new List<string>
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Environment.CurrentDirectory
            };

            var executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(executingAssemblyLocation))
            {
                var directory = Path.GetDirectoryName(executingAssemblyLocation);
                if (!string.IsNullOrEmpty(directory) && !baseDirectories.Contains(directory))
                {
                    baseDirectories.Add(directory);
                }
            }

            var entryAssemblyLocation = Assembly.GetEntryAssembly()?.Location;
            if (!string.IsNullOrEmpty(entryAssemblyLocation))
            {
                var directory = Path.GetDirectoryName(entryAssemblyLocation);
                if (!string.IsNullOrEmpty(directory) && !baseDirectories.Contains(directory))
                {
                    baseDirectories.Add(directory);
                }
            }

            // Combine the relative path with each base directory
            foreach (var baseDirectory in baseDirectories)
            {
                if (string.IsNullOrEmpty(baseDirectory))
                    continue;

                var combinedPath = Path.Combine(baseDirectory, path);
                var fullPath = Path.GetFullPath(combinedPath);

                if (File.Exists(fullPath) || Directory.Exists(fullPath))
                    return fullPath;
            }

            // As a fallback, return the path combined with the current directory
            return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path));
        }
    }
}
