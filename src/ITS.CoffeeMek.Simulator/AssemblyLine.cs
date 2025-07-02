using ITS.CoffeeMek.Models.QueueMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Simulator
{
    /// <summary>
    /// Passo 3
    /// </summary>
    public class AssemblyLine
    {
        public event EventHandler<TaskDoneEventArgs> TaskDone;

        public int StateId => _stateId;

        public int ProductsToComplete = 0;

        private readonly LatheMachine _latheMachine;

        private readonly int _machineId;

        private int _stateId = 1;

        private readonly IConfiguration _config;

        public AssemblyLine(IConfiguration config, int machineId, LatheMachine latheMachine)
        {
            _config = config;
            _machineId = machineId;
            _latheMachine = latheMachine;
        }

        public void Run()
        {
            Print($"Assembly Line running at: {DateTimeOffset.Now}");
            _latheMachine.TaskDone += LatheMachine_TaskDone;
            RunAsync();
        }

        public void Dispose()
        {
            _latheMachine.TaskDone -= LatheMachine_TaskDone;
            GC.SuppressFinalize(this);
        }

        private void LatheMachine_TaskDone(object? sender, TaskDoneEventArgs e)
        {
            ProductsToComplete++;
        }

        private async void RunAsync()
        {
            using HttpClient httpClient = new HttpClient();
            string uri = _config.GetValue<string>("assembly-line-post")!;
            float avgTime = 0;
            int waitsCount = 0;
            while (true)
            {
                GenerateState();
                var waitTime = Random.Shared.Next(100, 4000);
                await Task.Delay(waitTime);
                waitsCount++;
                avgTime = waitTime / 1000f / waitsCount;

                if (ProductsToComplete <= 0)
                {
                    continue;
                }

                var msg = new AssemblyLineDataQueueMessage
                {
                    MachineId = _machineId,
                    MachineStateId = _stateId,
                    LocalTimeStamp = DateTime.Now,
                    UTCTimeStamp = DateTime.UtcNow,
                    AvgStationTime = avgTime,
                    OperatorCount = waitTime > 3500 ? 2 : 1 // un operatore in più se ci mette troppo tempo
                };

                httpClient.PostAsJsonAsync(uri, msg);
                if (_stateId != 1) // se non è operativa, non si completa il prodotto
                {
                    continue;
                }
                ProductsToComplete--;
                Print($"Assembly line done at: {DateTimeOffset.Now}");
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

        private void Print(string message)
        {
            return;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
        }
    }
}
