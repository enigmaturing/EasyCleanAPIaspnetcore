using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EasyClean.API.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<UserRole> UserRoles { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
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
        public ICollection<MachineUsage> MachineUsages { get; set; }
        public ICollection<Topup> Topups { get; set; }
    }
}