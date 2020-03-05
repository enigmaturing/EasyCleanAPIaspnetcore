using System;

namespace EasyClean.API.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public double PricePaid { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public string Machine { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}