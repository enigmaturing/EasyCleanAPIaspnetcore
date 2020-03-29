using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IConfiguration config;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IMapper mapper;

        public AuthController(IConfiguration config,
                              UserManager<User> userManager, SignInManager<User> signInManager,
                              IMapper mapper)
        {
            this.config = config;  // Inject IConfiguration from Startup.cs, so we can retrieve our token in method login
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = this.mapper.Map<User>(userForRegisterDto); // Map a user from the recived dto
            var result = await this.userManager.CreateAsync(userToCreate, "password"); // set "password" as default password

            if (result.Succeeded)
            {
                var userCreated = userManager.FindByEmailAsync(userForRegisterDto.Email).Result;
                result = await userManager.AddToRolesAsync(userCreated, new[] {"Employee"});
                // Return 201 that means CreatedAtRoute, meaning that the user was created
                // and that it is available in a certain route of the API.
                if (result.Succeeded)
                    return StatusCode(201); 
                // ToDo: Return not only the code, but also the route where the user is available
                // ToDo: Return the user with the response too, mapped to a userForDetailedDto -> v.204
            }
            
            return BadRequest(result.Errors); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // Make use of ASP.NET Core Identity with UserManager and SigningManager:
            // UserManager:  Gives us the ability to store and retrieve Users in our DB.
            // SigningManager: Gives us the ability to check the user’s password and log the user in.
            var user = await userManager.FindByEmailAsync(userForLoginDto.Email.ToLower());

            if (user != null)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);
                if (result.Succeeded)
                {
                    // GenerateJwtToken is now an async method, so we need to AWAIT it
                    // Otherwise the SPA will try to login without having recieved the token yet
                    return Ok(new { token = await GenerateJwtToken(user) }); 
                }
            }
            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            // This method eturns a TOKEN when the user is logged in
            // The token can be validated by the server without making a DB call
            // This means that we can add bits of information to the token so that once
            // the user is validated by the server, these bits of information can be
            // retrieved witouht the need of acessing the DB. Those bits of information
            // are called claims. We build up a token that contains the User's Id and the
            // User's Email. We can define our claims as an array of objects of type Claim:
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Retrieve all roles for the given user
            var roles = await this.userManager.GetRolesAsync(user);

            // Loop through all the roles and add them one by one to
            // the JWT token that will be sent to the client when the given
            // user logs in to our api
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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