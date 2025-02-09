using Agentic.Profiles;

namespace Captain.Web.Client.Models
{
    public class UIClientSettings
    {
        public ClientSettings ClientSettings { get; set; } = new ClientSettings();
        public string ClientDisplayName { get; set; } = string.Empty;
    }
}