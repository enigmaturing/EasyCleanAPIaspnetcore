namespace EasyClean.API.Dtos
{
    public class MachineForListDto
    {
        public int Id { get; set; }
        public string LabeledAs { get; set; }
        public bool IsBlocked { get; set; }
        public string GroupName { get; set; }
        public string IconUrl { get; set; }
    }
}