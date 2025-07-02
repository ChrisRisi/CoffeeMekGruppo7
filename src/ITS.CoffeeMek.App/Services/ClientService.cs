using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class ClientService : IClientService
    {
        private readonly HttpClient _httpClient;

        public ClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Client>?> GetClientsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Client>?>("api/clients");
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Client?>($"api/clients/{id}");
        }

        public async Task<Client?> AddClientAsync(Client client)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Client?>();
        }

        public async Task UpdateClientAsync(Client client)
        {
            await _httpClient.PutAsJsonAsync($"api/clients/{client.Id}", client);
        }

        public async Task DeleteClientAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/clients/{id}");
        }
    }
}
