namespace ITS.CoffeeMek.Simulator
{
    public class Program
    {
        private static MillingMachine _millingMachine = null!;
        private static LatheMachine _latheMachine = null!;
        private static AssemblyLine _assemblyLine = null!;
        private static TestingLine _testingLine = null!;

        /// <summary>
        /// Prodotti da produrre.
        /// Non alzare troppo il numero, altrimenti si paga su Azure!
        /// </summary>
        private static int _productsToProduce = 50;

        public static void Main(string[] args)
        {
            // TODO: OGNI MACCHINA DEVE INVIARE ANCHE I DATI ALL'API

            // cose da fare:
            // limitare la produzione, quindi dopo che le macchine hanno inviato n dati, basta sennò pago su azure!!


            var site = args.Length > 0 ? args[0] : "italy";

            var builder = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            if (site == "brasil")
            {
                Console.WriteLine("Starting simulation for Brazil...");
                _millingMachine = new MillingMachine(config, 2, _productsToProduce);
                _latheMachine = new LatheMachine(config, 5, _millingMachine);
                _assemblyLine = new AssemblyLine(config, 8, _latheMachine);
                _testingLine = new TestingLine(config, 11, _assemblyLine);
            }
            else if (site == "vietnam")
            {
                Console.WriteLine("Starting simulation for Vietnam...");
                _millingMachine = new MillingMachine(config, 3, _productsToProduce);
                _latheMachine = new LatheMachine(config, 6, _millingMachine);
                _assemblyLine = new AssemblyLine(config, 9, _latheMachine);
                _testingLine = new TestingLine(config, 12, _assemblyLine);
            }
            else
            {
                // default: italia
                site = "italy";
                Console.WriteLine("Starting simulation for Italy...");
                _millingMachine = new MillingMachine(config, 1, _productsToProduce);
                _latheMachine = new LatheMachine(config, 4, _millingMachine);
                _assemblyLine = new AssemblyLine(config, 7, _latheMachine);
                _testingLine = new TestingLine(config, 10, _assemblyLine);
            }

            _millingMachine.Run();
            _latheMachine.Run();
            _assemblyLine.Run();
            _testingLine.Run();

            ShowStats(site);

            Console.ReadKey();
        }

        public static async Task ShowStats(string site)
        {
            Console.Clear();
            Console.WriteLine("Coffee Mek Simulator - Production Status");
            Console.WriteLine(site.ToUpper());
            Console.WriteLine("Press any key to stop the simulation...");
            Console.WriteLine("STATI MACCHINE:");
            PrintMachineStatus(1); Console.Write("Operativa  ");
            PrintMachineStatus(2); Console.Write("Manutenzione  ");
            PrintMachineStatus(3); Console.Write("Ferma  ");
            PrintMachineStatus(4); Console.WriteLine("Errore");
            PrintMachineStatus(5); Console.Write("Standby  ");
            PrintMachineStatus(6); Console.Write("Test passato  ");
            PrintMachineStatus(7); Console.WriteLine("Test fallito");
            Console.WriteLine();

            while (_testingLine.TotalProductsCompleted < _productsToProduce)
            {
                await Task.Delay(100);

                var progressBarMilling = CreateProgressBar(_millingMachine.ProductsToProduce * 20 / _productsToProduce, 20);
                var progressBarLathe = CreateProgressBar(Math.Min(20, _latheMachine.ProductsToComplete), 20);
                var progressBarAssembly = CreateProgressBar(Math.Min(20, _assemblyLine.ProductsToComplete), 20);
                var progressBarTesting = CreateProgressBar(Math.Min(20, _testingLine.ProductsToComplete), 20);
                var progressBarTotal = CreateProgressBar((_productsToProduce - _testingLine.TotalProductsCompleted) * 20 / _productsToProduce, 20, "II");

                Console.CursorVisible = false;
                Console.SetCursorPosition(0, 7);
                Console.WriteLine($"Products left to complete: {_productsToProduce - _testingLine.TotalProductsCompleted} / {_productsToProduce}       ");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"Percentage Left: \t{progressBarTotal}\n");
                PrintMachineStatus(_millingMachine.StateId);
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Milling Machine: {_millingMachine.ProductsToProduce * 100 / _productsToProduce}%   {progressBarMilling} ");
                Console.BackgroundColor = ConsoleColor.Black;
                PrintMachineStatus(_latheMachine.StateId);
                Console.WriteLine($"Lathe Machine:\t{_latheMachine.ProductsToComplete}    {progressBarLathe}");
                PrintMachineStatus(_assemblyLine.StateId);
                Console.WriteLine($"Assembly Line:\t{_assemblyLine.ProductsToComplete}    {progressBarAssembly}");
                PrintMachineStatus(_testingLine.StateId);
                Console.WriteLine($"Testing Line: \t{_testingLine.ProductsToComplete}    {progressBarTesting}  ");
                Console.WriteLine($"\nPassed products: {_testingLine.ProductsPassed}");
                Console.WriteLine($"Failed products: {_testingLine.ProductsFailed}");
            }
            Console.WriteLine("Production is done.");
        }

        private static void PrintMachineStatus(int statusId)
        {
            if (statusId == 1) Console.BackgroundColor = ConsoleColor.Green; // operativa
            else if (statusId == 2) Console.BackgroundColor = ConsoleColor.Yellow; // manutenzione
            else if (statusId == 3) Console.BackgroundColor = ConsoleColor.Gray; // ferma
            else if (statusId == 4) Console.BackgroundColor = ConsoleColor.Red; // errore
            else if (statusId == 5) Console.BackgroundColor = ConsoleColor.Cyan; // standby
            else if (statusId == 6) Console.BackgroundColor = ConsoleColor.Green; // prodotto passato
            else if (statusId == 7) Console.BackgroundColor = ConsoleColor.DarkGreen; // prodotto fallito
            else Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" ");
        }

        private static string CreateProgressBar(int count, int extra = 0, string progressPart = "[]")
        {
            string progressBar = string.Empty;

            // string builder? Mai sentito parlarne
            for (int i = 0; i < count; i++)
            {
                progressBar += progressPart;
            }
            for (int i = 0; i < extra - count; i++)
            {
                progressBar += "  ";
            }

            return progressBar;
        }
    }
}