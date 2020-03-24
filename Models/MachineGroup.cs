using System.Collections.Generic;

namespace EasyClean.API.Models
{
    public class MachineGroup
    {
        public int Id { get; set; }
        public virtual ICollection<Machine> Machines { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public virtual ICollection<Tariff> Tariffs { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public string IconUrl { get; set; }
        public string TypeName { get; set; }    
    }
}