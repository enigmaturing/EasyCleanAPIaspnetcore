using System.Collections.Generic;

namespace EasyClean.API.Models
{
    public class MachineGroup
    {
        public int Id { get; set; }
        public ICollection<Machine> Machines { get; set; }
        public ICollection<Tariff> Tariffs { get; set; }
        public string IconUrl { get; set; }
        public string TypeName { get; set; }    
    }
}