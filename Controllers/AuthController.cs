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
    [OpenApiTag("Auth", Description = "Registers and logs users in.")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;

        public AuthController(IAuthRepository repo)
        {
            this.repo = repo;
        }

        // POST: api/Auth/register/employee
        /// <summary>
        /// Registers a new user in the api and assigns employee roles to it, as specified in DTO.
        /// (Requires roles: Admin or Developer)
        /// </summary>
        /// <param name="userForRegisteEmployeeDto">Information about the user that wants to be registered.</param>
        /// <response code="201">Created.</response>        
        /// <response code="400">It was not possible to register the user. Email alreday taken.</response>
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>    
        [HttpPost("register/employee")]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterEmployee(UserForRegisterEmployeeDto userForRegisteEmployeeDto)
        {
            var user = await this.repo.RegisterEmployee(userForRegisteEmployeeDto);

            if (user != null)
            {
                return StatusCode(201);
                // ToDo: Return not only the code, but also the route where the user is available
                // ToDo: Return the user with the response too, mapped to a userForDetailedDto -> v.204
            }
            
            return BadRequest("Not possible to register the user. Try with another email."); 
        }

        // POST: api/Auth/register/client
        /// <summary>
        /// Registers a new user in the api and assigns client role to it.
        /// (Allows anonymous access)
        /// </summary>
        /// <param name="userForRegisterClientDto">Information about the user that wants to be registered.</param>
        /// <response code="201">Created.</response>        
        /// <response code="400">It was not possible to register the user. Email alreday taken.</response>
        [HttpPost("register/client")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterClient(UserForRegisterClientDto userForRegisterClientDto)
        {
            var user = await this.repo.RegisterClient(userForRegisterClientDto);

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
        /// Logs an already registered user in the api.
        /// (Allows anonymous access)
        /// </summary>
        /// <param name="userForLoginDto">Email and password of the user that logs in.</param>
        /// <response code="201">OK.</response>        
        /// <response code="401">Unauthorized to get token. Wrong email or password.</response>  
        [HttpPost("login")]
        [AllowAnonymous]
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