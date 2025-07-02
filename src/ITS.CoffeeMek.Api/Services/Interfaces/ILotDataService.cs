using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for Lot data operations
    /// </summary>
    public interface ILotDataService
    {
        /// <summary>
        /// Gets all lots
        /// </summary>
        /// <returns>Collection of lots</returns>
        Task<IEnumerable<Lot>> GetLotsAsync();
        
        /// <summary>
        /// Gets a lot by its ID
        /// </summary>
        /// <param name="id">The lot ID</param>
        /// <returns>The lot if found, null otherwise</returns>
        Task<Lot?> GetLotByIdAsync(int id);
        
        /// <summary>
        /// Inserts a new lot
        /// </summary>
        /// <param name="lot">The lot to insert</param>
        /// <returns>The ID of the newly created lot</returns>
        Task<int> InsertAsync(Lot lot);
        
        /// <summary>
        /// Updates an existing lot
        /// </summary>
        /// <param name="lot">The lot with updated values</param>
        Task UpdateAsync(Lot lot);
        
        /// <summary>
        /// Deletes a lot
        /// </summary>
        /// <param name="id">The ID of the lot to delete</param>
        Task DeleteAsync(int id);

        /// <summary>
        /// Gets all lots without detailed joins. Used for internal calculations.
        /// </summary>
        /// <returns>Collection of all lots</returns>
        Task<IEnumerable<Lot>> GetAllLotsAsync();

        /// <summary>
        /// Gets all lots for a specific order.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <returns>Collection of lots for the given order.</returns>
        Task<IEnumerable<Lot>> GetLotsByOrderIdAsync(int orderId);

        /// <summary>
        /// Gets all lots that are not assigned to any order.
        /// </summary>
        /// <returns>Collection of unassigned lots.</returns>
        Task<IEnumerable<Lot>> GetUnassignedLotsAsync();

        /// <summary>
        /// Gets all lots for a specific site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <returns>Collection of lots for the given site.</returns>
        Task<IEnumerable<Lot>> GetLotsBySiteIdAsync(int siteId);


    }
}