using ITS.CoffeeMek.Models.QueueMessages;
using System.Net.Http.Json;

namespace ITS.CoffeeMek.Simulator
{
    /// <summary>
    /// Tornio per le seghe. Passo 2
    /// </summary>
    public class LatheMachine : IDisposable
    {
        public event EventHandler<TaskDoneEventArgs> TaskDone;

        public int StateId => _stateId;

        public int ProductsToComplete = 0;

        private readonly MillingMachine _millingMachine;

        private readonly int _machineId;

        private int _stateId = 1;

        private readonly IConfiguration _config;

        public LatheMachine(IConfiguration config, int machineId, MillingMachine millingMachine)
        {
            _config = config;
            _machineId = machineId;
            _millingMachine = millingMachine;
        }

        public void Run()
        {
            Print($"Lathe Machine running at: {DateTimeOffset.Now}");
            _millingMachine.TaskDone += MillingMachine_TaskDone;
            RunAsync();
        }

        public void Dispose()
        {
            _millingMachine.TaskDone -= MillingMachine_TaskDone;
            GC.SuppressFinalize(this);
        }

        private void MillingMachine_TaskDone(object? sender, TaskDoneEventArgs e)
        {
            ProductsToComplete++;
        }

        private async void RunAsync()
        {
            using HttpClient httpClient = new HttpClient();
            string uri = _config.GetValue<string>("lathe-machine-post")!;
            while (true)
            {
                GenerateState();
                await Task.Delay(Random.Shared.Next(100, 4000));

                if (ProductsToComplete <= 0)
                {
                    continue;
                }

                var msg = new LatheDataQueueMessage
                {
                    MachineId = _machineId,
                    MachineStateId = _stateId,
                    LocalTimeStamp = DateTime.Now,
                    UTCTimeStamp = DateTime.UtcNow,
                    RotationSpeed = GetRotationSpeed(),
                    SpindleTemperature = GetTemperature()
                };

                httpClient.PostAsJsonAsync(uri, msg);

                if (_stateId != 1) // se non è operativa, non si completa il prodotto
                {
                    continue;
                }
                ProductsToComplete--;
                Print($"Lathe machine done at: {DateTimeOffset.Now}");
                TaskDone?.Invoke(this, new TaskDoneEventArgs());
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

        private double GetRotationSpeed()
        {
            if (_stateId != 1)
            {
                return 0;
            }

            return Random.Shared.Next(950, 1050);
        }

        private double GetTemperature()
        {
            if (_stateId != 1)
            {
                return 0;
            }

            return GetRotationSpeed() * 60 / 1000; // temp a 1000 rotazioni = 60 gradi
        }

        private void Print(string message)
        {
            return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
        }
    }
}
