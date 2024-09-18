using System.Collections.Generic;

namespace Agentic.Embeddings.Content
{
    public interface IContentProcessor
    {
        void SetChunkSize(int chunkSize);
        void ProcessFiles(IEnumerable<string> filePaths);
        void ProcessFolders(IEnumerable<string> folderPaths);
    }
}
