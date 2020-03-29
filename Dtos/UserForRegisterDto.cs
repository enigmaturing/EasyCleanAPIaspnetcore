using System.ComponentModel.DataAnnotations;

namespace EasyClean.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required(ErrorMessage="You must provide an email-address")]
        [EmailAddress(ErrorMessage="Correct the email address you entered. It does not have a valid email-format")]
        public string Email { get; set; }

        public string UserName { get; set; }    
    }
}