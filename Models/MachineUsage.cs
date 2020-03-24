using System;

namespace EasyClean.API.Models
{
    public class MachineUsage
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int QuantityOfServicesBooked { get; set; }
        public Machine Machine { get; set; }
        public int MachineId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Tariff Tariff { get; set; }
        public int TariffId { get; set; }
    }
}