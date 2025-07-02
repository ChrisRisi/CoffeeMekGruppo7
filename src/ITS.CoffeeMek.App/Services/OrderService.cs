using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Order>?> GetOrdersAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Order>?>("api/orders");
        }

        public async Task<IEnumerable<Order>?> GetOpenOrdersAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Order>?>("api/orders/open");
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Order?>($"api/orders/{id}");
        }

        public async Task<Order?> AddOrderAsync(Order order)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", order);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Order?>();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _httpClient.PutAsJsonAsync($"api/orders/{order.Id}", order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/orders/{id}");
        }
    }
}
