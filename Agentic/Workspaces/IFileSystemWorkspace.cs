namespace Agentic.Workspaces
{
    public interface IFileSystemWorkspace : IWorkspace
    {
        string BasePath { get; }

        string GetPath(string path);
    }
}