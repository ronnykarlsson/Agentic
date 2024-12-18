using Agentic.Agents;
using Agentic.Profiles;

namespace Agentic
{
    public interface IAgenticFactory
    {
        IChatAgent Create(AgenticProfile profile);
        IChatAgent CreateFromString(string yamlProfile);
        IChatAgent CreateFromFile(string path);
    }
}