using Agentic.Agents;
using Agentic.Profiles;

namespace Agentic
{
    public interface IAgenticFactory
    {
        IChatAgent Create(AgenticProfile profile);
        IChatAgent Create(AgenticProfile profile, ClientSettings clientSettings);
        IChatAgent CreateFromString(string yamlProfile);
        IChatAgent CreateFromString(string yamlProfile, ClientSettings clientSettings);
        IChatAgent CreateFromFile(string path);
    }
}