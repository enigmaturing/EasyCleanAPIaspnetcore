using System.ComponentModel.DataAnnotations;

namespace EasyClean.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required(ErrorMessage="You must provide an email-address")]
        [EmailAddress(ErrorMessage="Correct the email address you entered. It does not have a valid email-format")]
        public string Email { get; set; }

        [Required(ErrorMessage="You must provide a password")]
        [StringLength(15, MinimumLength=4, ErrorMessage="You must enter a password between 4 and 15 characters")]
        public string Password { get; set; }

        public string UserName { get; set; }    
    }
}