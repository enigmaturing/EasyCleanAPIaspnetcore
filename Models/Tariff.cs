namespace EasyClean.API.Models
{
    public class Tariff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsActive { get; set; }
        public MachineGroup MachineGroup { get; set; }
        public int MachineGroupId { get; set; }
    }
}