using Roast_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Models
{
    class RoasterStatus
    {
        public decimal CurrentTemperature { get; set; }
        public decimal CurrentHoldTemperature { get; set; }
        public bool IsHoldOn { get; set; }
        public bool IsHeaterOn { get; set; }
        public bool IsFanOn { get; set; }
        public bool IsProfileRunning { get; set; }
        public double ProfileElapsedSeconds { get; set; }
        public List<RoastProfile> RoastProfile { get; set; }
    }
}
