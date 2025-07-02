using ITS.CoffeeMek.App.Services.Interfaces;
using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services
{
    public class TestingLineService : ITestingLineService
    {
        private readonly HttpClient _httpClient;

        public TestingLineService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TestingLine>?> GetTestingLinesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<TestingLine>?>("api/testinglines");
        }

        public async Task<TestingLine?> GetTestingLineByMachineIdAsync(int machineId)
        {
            return await _httpClient.GetFromJsonAsync<TestingLine?>($"api/testinglines/{machineId}");
        }

        public async Task PostTestingLineAsync(TestingLineDataQueueMessage message)
        {
            var response = await _httpClient.PostAsJsonAsync("api/testinglines", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TestingLine>> GetTestingLineHistoryAsync(int machineId)
        {
            var history = await _httpClient.GetFromJsonAsync<IEnumerable<TestingLine>>($"api/testinglines/{machineId}/history");
            return history ?? Enumerable.Empty<TestingLine>();
        }
    }
}
