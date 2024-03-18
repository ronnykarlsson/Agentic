using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace Agentic.Profiles
{
    public class ProfileLoader : IProfileLoader
    {
        public AgenticProfile LoadProfileFromFile(string path)
        {
            var yamlProfile = File.ReadAllText(path);
            return LoadProfileFromString(yamlProfile);
        }

        public AgenticProfile LoadProfileFromString(string yamlProfile)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var result = deserializer.Deserialize<AgenticProfile>(yamlProfile);
            return result;
        }
    }
}
