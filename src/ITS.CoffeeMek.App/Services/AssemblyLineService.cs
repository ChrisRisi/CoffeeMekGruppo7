using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class AssemblyLineService : IAssemblyLineService
    {
        private readonly HttpClient _httpClient;

        public AssemblyLineService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AssemblyLine>?> GetAssemblyLinesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AssemblyLine>?>("api/assemblylines");
        }

        public async Task<AssemblyLine?> GetAssemblyLineByMachineIdAsync(int machineId)
        {
            return await _httpClient.GetFromJsonAsync<AssemblyLine?>($"api/assemblylines/{machineId}");
        }

        public async Task PostAssemblyLineAsync(AssemblyLineDataQueueMessage message)
        {
            var response = await _httpClient.PostAsJsonAsync("api/assemblylines", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<AssemblyLine>> GetAssemblyLineHistoryAsync(int machineId)
        {
            var history = await _httpClient.GetFromJsonAsync<IEnumerable<AssemblyLine>>($"api/assemblylines/{machineId}/history");
            return history ?? Enumerable.Empty<AssemblyLine>();
        }
    }
}
