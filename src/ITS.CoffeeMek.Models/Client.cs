using System;
using System.Collections.Generic;

namespace ITS.CoffeeMek.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
    }
}
