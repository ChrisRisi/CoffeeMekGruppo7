using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class MachineService : IMachineService
    {
        private readonly HttpClient _httpClient;

        public MachineService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Machine>?> GetMachinesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Machine>?>("api/machines");
        }

        public async Task<Machine?> GetMachineByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Machine?>($"api/machines/{id}");
        }

        public async Task<Machine?> AddMachineAsync(Machine machine)
        {
            var response = await _httpClient.PostAsJsonAsync("api/machines", machine);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Machine?>();
        }

        public async Task UpdateMachineAsync(Machine machine)
        {
            await _httpClient.PutAsJsonAsync($"api/machines/{machine.Id}", machine);
        }

        public async Task DeleteMachineAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/machines/{id}");
        }
    }
}
