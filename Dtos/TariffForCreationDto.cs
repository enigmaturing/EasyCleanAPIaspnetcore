namespace EasyClean.API.Dtos
{
    public class TariffForCreationDto
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsActive { get; set; }
        public int MachineGroupId { get; set; }
    }
}