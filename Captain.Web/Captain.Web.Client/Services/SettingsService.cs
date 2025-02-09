using Microsoft.JSInterop;
using System.Text.Json;
using Captain.Web.Client.Models;

namespace Captain.Web.Client.Services
{
    public class SettingsService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string SettingsKey = "settings_clients";
        private static readonly List<UIClientSettings> DefaultClients = new List<UIClientSettings>();

        public SettingsService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<List<UIClientSettings>> LoadSettingsAsync()
        {
            try
            {
                var settingsJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", SettingsKey);
                if (string.IsNullOrWhiteSpace(settingsJson))
                {
                    return DefaultClients;
                }

                var uiClients = JsonSerializer.Deserialize<List<UIClientSettings>>(settingsJson);
                return uiClients ?? DefaultClients;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings from local storage: {ex}");
                return DefaultClients;
            }
        }

        public async Task SaveSettingsAsync(List<UIClientSettings> uiClients)
        {
            var settingsJson = JsonSerializer.Serialize(uiClients);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", SettingsKey, settingsJson);
        }
    }
}