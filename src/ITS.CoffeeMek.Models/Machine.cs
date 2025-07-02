using System;
using System.Collections.Generic;

namespace ITS.CoffeeMek.Models
{
    public class Machine
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SiteId { get; set; }
        public string? Address { get; set; }

        public Site Site { get; set; } = null!;
    }
}
