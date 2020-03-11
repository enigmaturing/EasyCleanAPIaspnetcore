using System;

namespace EasyClean.API.Models
{
    public class Topup
    {
         public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime DateOfTopup { get; set; }
        public string NameOfEmployee { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }
}