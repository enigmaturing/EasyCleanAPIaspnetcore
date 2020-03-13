using System;

namespace EasyClean.API.Dtos
{
    public class MachineUsageForListDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string MachineLabeledAs { get; set; }
        public int UserId { get; set; }
        public string IconUrl { get; set; }
        public double TotalAmountPaid { get; set; }
        public int TotalDurationInMinutes { get; set; }
    }
}