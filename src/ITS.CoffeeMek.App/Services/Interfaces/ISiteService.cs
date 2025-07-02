using ITS.CoffeeMek.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface ISiteService
    {
        Task<IEnumerable<Site>?> GetSitesAsync();
        Task<Site?> GetSiteByIdAsync(int id);
        Task<Site?> AddSiteAsync(Site site);
        Task UpdateSiteAsync(Site site);
        Task DeleteSiteAsync(int id);
    }
}
