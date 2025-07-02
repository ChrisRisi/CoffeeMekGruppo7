using ITS.CoffeeMek.Models.QueueMessages;
using System.Net.Http.Json;

namespace ITS.CoffeeMek.Simulator
{
    /// <summary>
    /// Passo 4
    /// </summary>
    public class TestingLine
    {
        public event EventHandler<TaskDoneEventArgs> TaskDone;

        public int StateId => _stateId;

        public int ProductsToComplete = 0;

        public int TotalProductsCompleted => ProductsPassed + ProductsFailed;

        public int ProductsPassed = 0;

        public int ProductsFailed = 0;

        private readonly AssemblyLine _assemblyLine;

        private readonly int _machineId;

        private int _stateId = 1;

        private readonly IConfiguration _config;

        public TestingLine(IConfiguration config, int machineId, AssemblyLine assemblyLine)
        {
            _config = config;
            _machineId = machineId;
            _assemblyLine = assemblyLine;
        }

        public void Run()
        {
            Print($"Testing line running at: {DateTimeOffset.Now}");
            _assemblyLine.TaskDone += AssemblyLine_TaskDone;
            RunAsync();
        }

        public void Dispose()
        {
            _assemblyLine.TaskDone -= AssemblyLine_TaskDone;
            GC.SuppressFinalize(this);
        }

        private async void AssemblyLine_TaskDone(object? sender, TaskDoneEventArgs e)
        {
            ProductsToComplete++;
        }

        private async void RunAsync()
        {
            using HttpClient httpClient = new HttpClient();
            string uri = _config.GetValue<string>("testing-line-post")!;
            while (true)
            {
                await Task.Delay(Random.Shared.Next(100, 4000));

                if (ProductsToComplete <= 0)
                {
                    continue;
                }

                float boilerTemperature = Random.Shared.NextSingle() * 200;
                var boilerPressure = boilerTemperature * 5 / 200 + Random.Shared.NextSingle() / 10;
                var energyConsumption = boilerTemperature * 0.2 / 200 + Random.Shared.NextSingle() / 100;
                GenerateState(boilerTemperature);

                if (_stateId >= 2 && _stateId <= 5)
                {
                    // se lo stato è manutenzione, ferma, errore o standby, non si ha nulla
                    boilerTemperature = 0;
                    boilerPressure = 0;
                    energyConsumption = 0;
                }

                var msg = new TestingLineDataQueueMessage
                {
                    MachineId = _machineId,
                    MachineStateId = _stateId,
                    LocalTimeStamp = DateTime.Now,
                    UTCTimeStamp = DateTime.UtcNow,
                    BoilerPressure = boilerPressure,
                    BoilerTemperature = boilerTemperature,
                    EnergyConsumption = energyConsumption
                };

                if (msg.MachineStateId == 6) // stato "NON PASSATO" todo: verificare se è l'id corretto
                    ProductsPassed++;
                else if (msg.MachineStateId == 7)
                    ProductsFailed++;

                httpClient.PostAsJsonAsync(uri, msg);

                if (_stateId != 6 && _stateId != 7)
                {
                    continue;
                }
                ProductsToComplete--;
                Print($"Testing line done at: {DateTimeOffset.Now}");
                TaskDone?.Invoke(this, new TaskDoneEventArgs());
            }
        }

        private void GenerateState(float boilerTemperature)
        {
            // come funziona la generazione di ogni stato:
            // - se un numero random è sotto una certa soglia, allora lo stato corrente cambia
            // - se non è così, allora si controlla che lo stato corrente sia lo stesso di quello che si
            //   sta cercando di cambiare, e si tenta di rimanere con quello in base ad una scelta casuale
            // (la seconda condizione serve per simulare che la macchina rimanga nello stesso stato per un po' di tempo)

            if (boilerTemperature < 60)
            {
                _stateId = 7; // test fallito
                return;
            }

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

            _stateId = 6; // test passato
        }

        private void Print(string message)
        {
            return;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }
    }
}
