using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class MillingMachineService : IMillingMachineService
    {
        private readonly HttpClient _httpClient;

        public MillingMachineService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<MillingMachine>?> GetMillingMachinesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<MillingMachine>?>("api/millingmachines");
        }

        public async Task<MillingMachine?> GetMillingMachineByMachineIdAsync(int machineId)
        {
            return await _httpClient.GetFromJsonAsync<MillingMachine?>($"api/millingmachines/{machineId}");
        }

        public async Task PostMillingMachineAsync(MillingMachineDataQueueMessage message)
        {
            var response = await _httpClient.PostAsJsonAsync("api/millingmachines", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<MillingMachine>> GetMillingMachineHistoryAsync(int machineId)
        {
            var history = await _httpClient.GetFromJsonAsync<IEnumerable<MillingMachine>>($"api/millingmachines/{machineId}/history");
            return history ?? Enumerable.Empty<MillingMachine>();
        }
    }
}
