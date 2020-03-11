using System;

namespace EasyClean.API.Dtos
{
    public class TopupsForDetailedDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime DateOfTopup { get; set; }
        public string NameOfEmployee { get; set; }
    }
}