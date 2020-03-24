using System;
using System.Collections.Generic;

namespace EasyClean.API.Models
{
    public class Machine
    {
        public int Id { get; set; }
        public string LabeledAs { get; set; }
        public ICollection<MachineUsage> MachineUsages { get; set; }
        public bool IsBlocked { get; set; }
        public MachineGroup MachineGroup { get; set; }
        public int MachineGroupId { get; set; }
    }
}