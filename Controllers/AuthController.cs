using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
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
            var user = await this.repo.Register(userForRegisterDto);

            if (user != null)
            {
                return StatusCode(201);
                // ToDo: Return not only the code, but also the route where the user is available
                // ToDo: Return the user with the response too, mapped to a userForDetailedDto -> v.204
            }
            
            return BadRequest("Not possible to register the user. Try with another email."); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var token = await this.repo.Login(userForLoginDto);
            if (token != null)
            {
                return Ok(new { token = token }); 
            }
            return Unauthorized("Wrong email or password");
        }
    }
}