using System.Threading.Tasks;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;

        public AuthController(IAuthRepository repo)
        {
            this.repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // ToDo: Validate request

            // Avoid having two users with the same email, one containing capital
            // letters and one not
            userForRegisterDto.Email = userForRegisterDto.Email.ToLower();
            if (await this.repo.UserExists(userForRegisterDto.Email))
                return BadRequest("This email was already used to register another account");
            
            // We can not assign the string password to userToCreate because the constructor
            // of the User class does not contain a porperty password of type string, but two
            // byte[] properties: passwordHash and passwordSalt. So, by the moment, we just
            // assign a value to the property Email and let the Register method of our repo
            // generate passwordHash and passwordSalt internally
            var userToCreate = new User(){ Email = userForRegisterDto.Email };
            var createdUser = this.repo.Register(userToCreate, userForRegisterDto.Password);

            // Return 201 that means CreatedAtRoute, meaning that the user was created
            // and that it is available in a certain route of the API.
            return StatusCode(201); // ToDo: Return not only the code, but also the route where the user is available
        }
    }
}