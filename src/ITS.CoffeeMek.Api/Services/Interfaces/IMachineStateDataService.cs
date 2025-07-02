using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for MachineState data operations
    /// </summary>
    public interface IMachineStateDataService
    {
        /// <summary>
        /// Gets all machine states
        /// </summary>
        /// <returns>Collection of machine states</returns>
        Task<IEnumerable<MachineState>> GetMachineStatesAsync();
        
        /// <summary>
        /// Gets a machine state by its ID
        /// </summary>
        /// <param name="id">The machine state ID</param>
        /// <returns>The machine state if found, null otherwise</returns>
        Task<MachineState?> GetMachineStateByIdAsync(int id);
        
        /// <summary>
        /// Inserts a new machine state
        /// </summary>
        /// <param name="machineState">The machine state to insert</param>
        /// <returns>The ID of the newly created machine state</returns>
        Task<int> InsertAsync(MachineState machineState);
        
        /// <summary>
        /// Updates an existing machine state
        /// </summary>
        /// <param name="machineState">The machine state with updated values</param>
        Task UpdateAsync(MachineState machineState);
        
        /// <summary>
        /// Deletes a machine state
        /// </summary>
        /// <param name="id">The ID of the machine state to delete</param>
        Task DeleteAsync(int id);
    }
}