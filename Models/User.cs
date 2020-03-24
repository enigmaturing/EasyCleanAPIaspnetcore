using System;
using System.Collections.Generic;

namespace EasyClean.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public int ZIP { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
        public double RemainingCredit { get; set; }
        public virtual ICollection<MachineUsage> MachineUsages { get; set; } // virtual: this is a navegation propery and needs to be lazy loaded
        public virtual ICollection<Topup> Topups { get; set; } // virtual: this is a navegation propery and needs to be lazy loaded
    }
}