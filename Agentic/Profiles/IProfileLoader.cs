namespace Agentic.Profiles
{
    public interface IProfileLoader
    {
        AgenticProfile LoadProfileFromFile(string path);
        AgenticProfile LoadProfileFromString(string yamlProfile);
    }
}