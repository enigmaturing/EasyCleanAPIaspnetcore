using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EasyClean.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration config;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AuthController(IAuthRepository repo, IConfiguration config,
                              UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.repo = repo;
            this.config = config;  // Inject IConfiguration from Startup.cs, so we can retrieve our token in method login
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
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
            var userToCreate = new User() { Email = userForRegisterDto.Email };
            var createdUser = this.repo.Register(userToCreate, userForRegisterDto.Password);

            // Return 201 that means CreatedAtRoute, meaning that the user was created
            // and that it is available in a certain route of the API.
            return StatusCode(201); // ToDo: Return not only the code, but also the route where the user is available
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // Make use of ASP.NET Core Identity with UserManager and SigningManager:
            // UserManager:  Gives us the ability to store and retrieve Users in our DB.
            // SigningManager: Gives us the ability to check the user’s password and log the user in.
            var user = await userManager.FindByEmailAsync(userForLoginDto.Email.ToLower());
            var result = await signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                return Ok(new { token = GenerateJwtToken(user) });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            // This method eturns a TOKEN when the user is logged in
            // The token can be validated by the server without making a DB call
            // This means that we can add bits of information to the token so that once
            // the user is validated by the server, these bits of information can be
            // retrieved witouht the need of acessing the DB. Those bits of information
            // are called claims. We build up a token that contains the User's Id and the
            // User's Email. We can define our claims as an array of objects of type Claim:
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // We need a key to sign the token that we will send to the client, so that we can check that the token
            // is a valid one when it comes back to the server. This key will be a hashed string so that it is not readable
            // We have stored the not-hashed version of this tokenKey in our file appsettings.json to keep it private.
            // To hash this string called tokenKey we will make use of the class SymmetricSecurityKey which accepts that
            // tokenKey only as an array of bytes encoded in UTF8 format
            var tokenKey = this.config.GetSection("AppSettings:TokenKey").Value;    // get the token from appsettings.json
            var tokenEncodedUtf8 = Encoding.UTF8.GetBytes(tokenKey);                // get bytes of token in UTF8
            var key = new SymmetricSecurityKey(tokenEncodedUtf8);                   // generate a hased key from those bytes

            // Generate signing credentials with our generated key as part of the signing credentials. This takes the scurity
            // key and the hashing algorithm that we will use to hash this particular key (SecurityAlgorithms.HmacSha512Signature)
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create a token descriptor, that will contain the following:
            // - Claims
            // - Signing credentials
            // - Expiry date for our token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials,
                Expires = DateTime.Now.AddDays(1)
            };

            // Create a token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Using our newly created token handler, we can create a token and pass him the token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}