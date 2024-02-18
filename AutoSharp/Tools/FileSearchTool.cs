using AutoSharp.Datastore;
using System.IO;
using System.Linq;

namespace AutoSharp.Tools
{
    public class FileSearchTool : ITool
    {
        public string Name { get; }
        public string ParameterName { get; }
        public string Description { get; }

        private readonly LuceneService _datastore;

        public FileSearchTool(string name, string description)
        {
            Name = name;
            ParameterName = "query";
            Description = description;

            _datastore = new LuceneService();
        }

        public void AddFile(string key, string path)
        {
            var fileContents = File.ReadAllText(path);
            _datastore.IndexText(key, fileContents);
        }

        public string Invoke(string parameter)
        {
            var searchResults = _datastore.SearchText(parameter);
            if (!searchResults.Any()) return "No results found.";
            return string.Join("\n", searchResults.Take(3).Select((o, i) => $"Result {i}:\n{o.ContentSnippet}"));
        }
    }
}
