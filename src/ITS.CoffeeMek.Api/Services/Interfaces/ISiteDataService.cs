using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.CoffeeMek.Models;

namespace ITS.CoffeeMek.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for Site data operations
    /// </summary>
    public interface ISiteDataService
    {
        /// <summary>
        /// Gets all sites
        /// </summary>
        /// <returns>Collection of sites</returns>
        Task<IEnumerable<Site>> GetSitesAsync();
        
        /// <summary>
        /// Gets a site by its ID
        /// </summary>
        /// <param name="id">The site ID</param>
        /// <returns>The site if found, null otherwise</returns>
        Task<Site?> GetSiteByIdAsync(int id);
        
        /// <summary>
        /// Inserts a new site
        /// </summary>
        /// <param name="site">The site to insert</param>
        /// <returns>The ID of the newly created site</returns>
        Task<int> InsertAsync(Site site);
        
        /// <summary>
        /// Updates an existing site
        /// </summary>
        /// <param name="site">The site with updated values</param>
        Task UpdateAsync(Site site);
        
        /// <summary>
        /// Deletes a site
        /// </summary>
        /// <param name="id">The ID of the site to delete</param>
        Task DeleteAsync(int id);
    }
}