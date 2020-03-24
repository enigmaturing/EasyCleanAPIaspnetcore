using System;

namespace EasyClean.API.Models
{
    public class Topup
    {
         public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime DateOfTopup { get; set; }
        public string NameOfEmployee { get; set; }
        public virtual User User { get; set; } // virtual: it is a navegation propery and needs to be lazy loaded
        public int UserId { get; set; }
    }
}