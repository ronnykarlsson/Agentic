using Agentic.DependencyInjection;
using Captain.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Captain.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAgenticServices();
            builder.Services.AddTransient<ChatService>();
            builder.Services.AddScoped<SettingsService>();
            builder.Services.AddScoped<AgentService>();

            await builder.Build().RunAsync();
        }
    }
}
