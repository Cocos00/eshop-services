using System;

namespace Catalog.API.Models
{
    public class Sensor
    {
        public Guid Id { get; set; }
        public double Distance { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
