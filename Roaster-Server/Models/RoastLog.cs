using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roast_Server.Models
{
    internal class RoastLog
    {
        public double FirstCrackSeconds { get; set; }
        public double SecondCrackSeconds { get; set; }
        public List<TimeTemperature> TimeTemperatureEntries { get; set; }

        public RoastLog()
        {
            TimeTemperatureEntries = new List<TimeTemperature>();
        }
    }

    struct TimeTemperature
    {
        public double Seconds { get; set; }
        public decimal Temperature { get; set; }
    }

}
