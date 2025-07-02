using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<Client>?> GetClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client?> AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(int id);
    }
}
