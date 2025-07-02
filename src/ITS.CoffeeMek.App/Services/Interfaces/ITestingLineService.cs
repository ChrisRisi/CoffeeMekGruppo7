using ITS.CoffeeMek.Models;
using ITS.CoffeeMek.Models.QueueMessages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.App.Services.Interfaces
{
    public interface ITestingLineService
    {
        Task<IEnumerable<TestingLine>?> GetTestingLinesAsync();
        Task<TestingLine?> GetTestingLineByMachineIdAsync(int machineId);
        Task PostTestingLineAsync(TestingLineDataQueueMessage message);
        Task<IEnumerable<TestingLine>> GetTestingLineHistoryAsync(int machineId);
    }
}
