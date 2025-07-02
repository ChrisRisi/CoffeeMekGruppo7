using ITS.CoffeeMek.Models.QueueMessages;
using System.Net.Http.Json;

namespace ITS.CoffeeMek.Simulator
{
    /// <summary>
    /// Fresa della minchia. Passo 1
    /// </summary>
    public class MillingMachine
    {
        public event EventHandler<TaskDoneEventArgs> TaskDone;

        public int StateId => _stateId;

        public int ProductsToProduce { get; private set; }

        private readonly int _machineId;

        private int _stateId = 1;

        private readonly IConfiguration _config;

        public MillingMachine(IConfiguration config, int machineId, int productsToProduce)
        {
            _config = config;
            _machineId = machineId;
            ProductsToProduce = productsToProduce;
        }

        public void Run()
        {
            RunAsync();
        }

        private async Task RunAsync()
        {
            using HttpClient httpClient = new HttpClient();
            string uri = _config.GetValue<string>("milling-machine-post")!;
            while (ProductsToProduce > 0)
            {
                Print($"Milling Machine running at: {DateTimeOffset.Now}");
                GenerateState();
                await Task.Delay(Random.Shared.Next(100, 4000));

                var msg = new MillingMachineDataQueueMessage
                {
                    MachineId = _machineId,
                    MachineStateId = _stateId,
                    LocalTimeStamp = DateTime.Now,
                    UTCTimeStamp = DateTime.UtcNow,
                    CycleTime = GetCycleTime(),
                    Vibration = GetVibration()
                };

                httpClient.PostAsJsonAsync(uri, msg);
                if (_stateId == 1)
                {
                    ProductsToProduce--;
                    Print($"Milling machine done at: {DateTimeOffset.Now}");
                    TaskDone?.Invoke(this, new TaskDoneEventArgs());
                }
            }
        }

        private void GenerateState()
        {
            // come funziona la generazione di ogni stato:
            // - se un numero random è sotto una certa soglia, allora lo stato corrente cambia
            // - se non è così, allora si controlla che lo stato corrente sia lo stesso di quello che si
            //   sta cercando di cambiare, e si tenta di rimanere con quello in base ad una scelta casuale
            // (la seconda condizione serve per simulare che la macchina rimanga nello stesso stato per un po' di tempo)

            if (Random.Shared.Next(0, 10) < 1 || (_stateId == 2 && Random.Shared.Next(0, 10) < 3))
            {
                _stateId = 2; // manutenzione
                return;
            }
            if (Random.Shared.Next(0, 15) < 1 || (_stateId == 3 && Random.Shared.Next(0, 12) < 3))
            {
                _stateId = 3; // ferma
                return;
            }
            if (Random.Shared.Next(0, 20) < 1 || (_stateId == 4 && Random.Shared.Next(0, 10) < 2))
            {
                _stateId = 4; // errore
                return;
            }
            if (Random.Shared.Next(0, 15) < 1 || (_stateId == 5 && Random.Shared.Next(0, 12) < 4))
            {
                _stateId = 5; // standby
                return;
            }

            _stateId = 1; // operativa
        }

        private double GetCycleTime()
        {
            if (_stateId != 1)
                return 0;

            return Random.Shared.Next(1000, 1100);
        }

        private double GetVibration()
        {
            if (_stateId != 1)
                return 0;

            return Math.Min(Random.Shared.NextDouble() / 10, 0.05);
        }

        private void Print(string message)
        {
            return;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }
    }
}
