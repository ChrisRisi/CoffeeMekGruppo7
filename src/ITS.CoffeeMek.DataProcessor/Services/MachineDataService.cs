using Dapper;
using ITS.CoffeeMek.DataProcessor.Data;
using ITS.CoffeeMek.DataProcessor.Services.Interfaces;
using ITS.CoffeeMek.Models;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.DataProcessor.Services
{
    public class MachineDataService : IMachineDataService
    {
        private readonly DapperContext _context;

        public MachineDataService(DapperContext context)
        {
            _context = context;
        }

        public async Task<Machine?> GetMachineByIdAsync(int machineId)
        {
            using var connection = _context.CreateConnection();
            const string query = "SELECT * FROM CoffeeMek.Machines WHERE Id = @machineId";
            return await connection.QuerySingleOrDefaultAsync<Machine>(query, new { machineId });
        }
    }
}
