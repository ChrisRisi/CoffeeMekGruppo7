using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for Machine data operations
    /// </summary>
    public interface IMachineDataService
    {
        /// <summary>
        /// Gets all machines
        /// </summary>
        /// <returns>Collection of machines</returns>
        Task<IEnumerable<Machine>> GetMachinesAsync();
        
        /// <summary>
        /// Gets a machine by its ID
        /// </summary>
        /// <param name="id">The machine ID</param>
        /// <returns>The machine if found, null otherwise</returns>
        Task<Machine?> GetMachineByIdAsync(int id);
        
        /// <summary>
        /// Inserts a new machine
        /// </summary>
        /// <param name="machine">The machine to insert</param>
        /// <returns>The ID of the newly created machine</returns>
        Task<int> InsertAsync(Machine machine);
        
        /// <summary>
        /// Updates an existing machine
        /// </summary>
        /// <param name="machine">The machine with updated values</param>
        Task UpdateAsync(Machine machine);
        
        /// <summary>
        /// Deletes a machine
        /// </summary>
        /// <param name="id">The ID of the machine to delete</param>
        Task DeleteAsync(int id);
    }
}