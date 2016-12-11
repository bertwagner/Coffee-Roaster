using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roaster_Server.Models.Database
{
    class Profile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        public string RoastSchedule { get; set; }
        public int? BeanGrams { get; set; }
    }
}
