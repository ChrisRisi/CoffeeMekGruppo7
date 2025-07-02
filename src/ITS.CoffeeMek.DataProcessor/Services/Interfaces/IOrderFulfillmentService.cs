using System.Threading.Tasks;

namespace ITS.CoffeeMek.DataProcessor.Services.Interfaces
{
    public interface IOrderFulfillmentService
    {
        Task ProcessOrderFulfillmentAsync();
    }
}
