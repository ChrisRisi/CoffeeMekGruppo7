using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class LotService : ILotService
    {
        private readonly HttpClient _httpClient;

        public LotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Lot>?> GetLotsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Lot>?>("api/lots");
        }

        public async Task<Lot?> GetLotByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Lot?>($"api/lots/{id}");
        }

        public async Task<Lot?> AddLotAsync(Lot lot)
        {
            var response = await _httpClient.PostAsJsonAsync("api/lots", lot);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Lot?>();
        }

        public async Task UpdateLotAsync(Lot lot)
        {
            await _httpClient.PutAsJsonAsync($"api/lots/{lot.Id}", lot);
        }

        public async Task DeleteLotAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/lots/{id}");
        }

        public async Task<IEnumerable<Lot>?> GetLotsBySiteIdAsync(int siteId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Lot>?>($"api/lots/site/{siteId}");
        }

        public async Task<IEnumerable<Lot>?> GetLotsByOrderIdAsync(int orderId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Lot>?>($"api/lots/order/{orderId}");
        }
    }
}
