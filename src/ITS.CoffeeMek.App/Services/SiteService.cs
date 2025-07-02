using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class SiteService : ISiteService
    {
        private readonly HttpClient _httpClient;

        public SiteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Site>?> GetSitesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Site>?>("api/sites");
        }

        public async Task<Site?> GetSiteByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Site?>($"api/sites/{id}");
        }

        public async Task<Site?> AddSiteAsync(Site site)
        {
            var response = await _httpClient.PostAsJsonAsync("api/sites", site);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Site?>();
        }

        public async Task UpdateSiteAsync(Site site)
        {
            await _httpClient.PutAsJsonAsync($"api/sites/{site.Id}", site);
        }

        public async Task DeleteSiteAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/sites/{id}");
        }
    }
}
