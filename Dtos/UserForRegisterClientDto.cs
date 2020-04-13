using System;
using System.ComponentModel.DataAnnotations;

namespace EasyClean.API.Dtos
{
    public class UserForRegisterClientDto
    {
        [Required(ErrorMessage = "You must provide an email-address")]
        [EmailAddress(ErrorMessage = "Correct the email address you entered. It does not have a valid email-format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must provide a password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "You must provide a surname")]
        public string Surname { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public int ZIP { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
    }
}

