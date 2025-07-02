using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for Client data operations
    /// </summary>
    public interface IClientDataService
    {
        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <returns>Collection of clients</returns>
        Task<IEnumerable<Client>> GetClientsAsync();
        
        /// <summary>
        /// Gets a client by its ID
        /// </summary>
        /// <param name="id">The client ID</param>
        /// <returns>The client if found, null otherwise</returns>
        Task<Client?> GetClientByIdAsync(int id);
        
        /// <summary>
        /// Inserts a new client
        /// </summary>
        /// <param name="client">The client to insert</param>
        /// <returns>The ID of the newly created client</returns>
        Task<int> InsertAsync(Client client);
        
        /// <summary>
        /// Updates an existing client
        /// </summary>
        /// <param name="client">The client with updated values</param>
        Task UpdateAsync(Client client);
        
        /// <summary>
        /// Deletes a client
        /// </summary>
        /// <param name="id">The ID of the client to delete</param>
        Task DeleteAsync(int id);
    }
}