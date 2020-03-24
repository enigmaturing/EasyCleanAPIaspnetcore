using System;

namespace EasyClean.API.Models
{
    public class MachineUsage
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int QuantityOfServicesBooked { get; set; }
        public virtual Machine Machine { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public int MachineId { get; set; }
        public virtual User User { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public int UserId { get; set; }
        public virtual Tariff Tariff { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public int TariffId { get; set; }
    }
}