using System;

namespace EasyClean.API.Dtos
{
    public class PurchasesForDetailedDto
    {
        public int Id { get; set; }
        public double PricePaid { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public string Machine { get; set; }
    }
}