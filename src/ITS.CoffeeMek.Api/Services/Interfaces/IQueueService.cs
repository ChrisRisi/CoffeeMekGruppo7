using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    public interface IQueueService
    {
        Task SendMessageAsync<T>(string queueName, T messagePayload);
    }
}
