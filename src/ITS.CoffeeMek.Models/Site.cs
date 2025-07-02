using System;
using System.Collections.Generic;

namespace ITS.CoffeeMek.Models
{
    public class Site
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public string? Address { get; set; }
    }
}
