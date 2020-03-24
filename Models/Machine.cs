using System;
using System.Collections.Generic;

namespace EasyClean.API.Models
{
    public class Machine
    {
        public int Id { get; set; }
        public string LabeledAs { get; set; }
        public virtual ICollection<MachineUsage> MachineUsages { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public bool IsBlocked { get; set; }
        public virtual MachineGroup MachineGroup { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public int MachineGroupId { get; set; }
    }
}