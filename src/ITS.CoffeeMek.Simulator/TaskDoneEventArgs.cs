using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CoffeeMek.Simulator
{
    /// <summary>
    /// Indica che il compito di una macchina è finito
    /// (ad esempio, una assembly line ha finito di assemblare un prodotto).
    /// </summary>
    public class TaskDoneEventArgs : EventArgs
    {
        public TaskDoneEventArgs()
        {
        }
    }
}
