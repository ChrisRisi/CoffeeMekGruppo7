using Azure.Messaging.ServiceBus;
using ITS.CoffeeMek.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services
{
    public class QueueService : IQueueService
    {
        private readonly ServiceBusClient _client;

        public QueueService(IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString") ?? configuration["ServiceBus:ConnectionString"];
            _client = new ServiceBusClient(connectionString);
        }

        public async Task SendMessageAsync<T>(string queueName, T messagePayload)
        {
            await using var sender = _client.CreateSender(queueName);
            var messageBody = JsonSerializer.Serialize(messagePayload);
            var message = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(message);
        }
    }
}
