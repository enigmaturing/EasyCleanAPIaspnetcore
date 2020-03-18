using System.Collections.Generic;
using EasyClean.API.Models;

namespace EasyClean.API.Dtos
{
    public class MachineGroupForListDto
    {
        public int Id { get; set; }
        public ICollection<Machine> Machines { get; set; }
        public ICollection<Tariff> Tariffs { get; set; }
        public string IconUrl { get; set; }
        public string TypeName { get; set; }    
    }
}