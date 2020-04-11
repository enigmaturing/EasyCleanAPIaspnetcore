using System.ComponentModel.DataAnnotations;

namespace EasyClean.API.Dtos
{
    public class UserForRegisterEmployeeDto
    {
        [Required(ErrorMessage = "You must provide an email-address")]
        [EmailAddress(ErrorMessage = "Correct the email address you entered. It does not have a valid email-format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must provide a surname")]
        public string Surname { get; set; }

        public string UserName { get; set; }

        public string[] RoleNames { get; set; }
    }
}
