using System;

namespace EasyClean.API.Dtos
{
    public class MachineUsageForDetailedDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string MachineLabeledAs { get; set; }
        public int UserId { get; set; }
        public string IconUrl { get; set; }
        public double TotalAmountPaid { get; set; }
        public int TotalDurationInMinutes { get; set; }
        public string TariffName { get; set; }
        public int QuantityOfServicesBooked { get; set; }
        public double PricePerServiceBooked { get; set; }
        public int DurationPerServiceBooked { get; set; }
    }
}