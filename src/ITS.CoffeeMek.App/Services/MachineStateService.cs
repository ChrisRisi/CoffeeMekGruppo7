using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class MachineStateService : IMachineStateService
    {
        private readonly HttpClient _httpClient;

        public MachineStateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<MachineState>?> GetMachineStatesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<MachineState>?>("api/machinestates");
        }

        public async Task<MachineState?> GetMachineStateByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<MachineState?>($"api/machinestates/{id}");
        }

        public async Task<MachineState?> AddMachineStateAsync(MachineState machineState)
        {
            var response = await _httpClient.PostAsJsonAsync("api/machinestates", machineState);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MachineState?>();
        }

        public async Task UpdateMachineStateAsync(MachineState machineState)
        {
            await _httpClient.PutAsJsonAsync($"api/machinestates/{machineState.Id}", machineState);
        }

        public async Task DeleteMachineStateAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/machinestates/{id}");
        }
    }
}
