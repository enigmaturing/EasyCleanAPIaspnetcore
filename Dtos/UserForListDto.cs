using System;

namespace EasyClean.API.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public DateTime LastActive { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public int ZIP { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
    }
}