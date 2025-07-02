using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class LatheService : ILatheService
    {
        private readonly HttpClient _httpClient;

        public LatheService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Lathe>?> GetLathesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Lathe>?>("api/lathes");
        }

        public async Task<Lathe?> GetLatheByMachineIdAsync(int machineId)
        {
            return await _httpClient.GetFromJsonAsync<Lathe?>($"api/lathes/{machineId}");
        }

        public async Task PostLatheAsync(LatheDataQueueMessage message)
        {
            var response = await _httpClient.PostAsJsonAsync("api/lathes", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Lathe>> GetLatheHistoryAsync(int machineId)
        {
            var history = await _httpClient.GetFromJsonAsync<IEnumerable<Lathe>>($"api/lathes/{machineId}/history");
            return history ?? Enumerable.Empty<Lathe>();
        }
    }
}
