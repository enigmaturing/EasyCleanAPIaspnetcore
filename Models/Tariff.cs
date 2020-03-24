namespace EasyClean.API.Models
{
    public class Tariff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsActive { get; set; }
        public virtual MachineGroup MachineGroup { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public int MachineGroupId { get; set; }
    }
}