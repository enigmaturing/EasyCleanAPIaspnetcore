using System;

namespace EasyClean.API.Dtos
{
    public class MachineUsageForCreationDto
    {
        public int MachineId { get; set; }
        public int UserId { get; set; }
        public int TariffId { get; set; }
        public int QuantityOfServicesBooked { get; set; }
    }
}
