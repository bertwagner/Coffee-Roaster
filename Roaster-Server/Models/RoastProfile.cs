using Roast_Server.Models;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Models
{
    class RoastProfile
    {
        public string Name { get; set; }
        public List<RoastSchedule> RoastSchedule { get; set; }
        public int? BeanGrams { get; set; }
    }
}
