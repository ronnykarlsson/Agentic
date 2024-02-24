using Agentic.Datastore;
using System.IO;
using System.Linq;

namespace Agentic.Tools
{
    public class FileSearchTool : ITool
    {
        public string Tool { get; }
        public string Description { get; }
        public ToolParameter<string> Query { get; set; }

        private readonly LuceneService _datastore;

        public FileSearchTool(string name, string description)
        {
            Tool = name;
            Description = description;

            _datastore = new LuceneService();
        }

        public void AddFile(string key, string path)
        {
            var fileContents = File.ReadAllText(path);
            _datastore.IndexText(key, fileContents);
        }

        public string Invoke()
        {
            var parameter = Query.Value;

            var searchResults = _datastore.SearchText(parameter);
            if (!searchResults.Any()) return "No results found.";
            return string.Join("\n", searchResults.Take(3).Select((o, i) => $"Result {i}:\n{o.ContentSnippet}"));
        }
    }
}
