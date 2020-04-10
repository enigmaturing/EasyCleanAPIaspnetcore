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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EasyClean.API.Controllers
{
    [OpenApiTag("Auth", Description = "Registers and logs users in")]
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

        // POST: api/Auth/register
        /// <summary>
        /// Registers a user in the api
        /// </summary>
        /// <param name="userForRegisterDto">Information about the user that wants to be registered</param>
        /// <response code="201">OK.</response>        
        /// <response code="400">It was not possible to register the user. Email alreday taken.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        // POST: api/Auth/login
        /// <summary>
        /// Logs a user in the api
        /// </summary>
        /// <param name="userForLoginDto">Email and password of the user that logs in</param>
        /// <response code="201">OK.</response>        
        /// <response code="401">Wrong email or password</response>  
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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