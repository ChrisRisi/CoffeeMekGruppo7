using System.Threading.Tasks;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    public interface IOrderFulfillmentService
    {
        Task ProcessOrderFulfillmentAsync();
    }
}
