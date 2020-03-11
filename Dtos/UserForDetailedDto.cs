using System;
using System.Collections.Generic;
using EasyClean.API.Models;

namespace EasyClean.API.Dtos
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
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
        public ICollection<PurchasesForDetailedDto> Purchases { get; set; }
        public ICollection<TopupsForDetailedDto> Topups { get; set; }
    }
}