using System.Collections.Generic;
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
    [OpenApiTag("Users", Description = "Retrieves and creates users.")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IEasyCleanRepository repo;
        private readonly IMapper mapper;

        public UsersController(IEasyCleanRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        // GET: api/Users
        /// <summary>
        /// Retrieves all users.
        /// (Requires roles: BackOfficeEmployee, Admin or Developer)
        /// </summary>
        /// <remarks>
        /// The returned user is mapped to a UserForListDto.
        /// </remarks>
        /// <response code="200">OK. Returns all users.</response>        
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>               
        /// <response code="404">NotFound. No users found.</response>        
        [HttpGet]
        [Authorize(Policy = "RequireBackOfficeRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await this.repo.GetUsers();
            if (users == null)
            {
                return NotFound();
            }
            var usersToReturnToClient = this.mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturnToClient);
        }

        // GET: api/Users/5
        /// <summary>
        /// Retrieves a single user by his id.
        /// (Requires no specific roles. User must be just logged in)
        /// </summary>
        /// <remarks>
        /// The returned user is mapped to a UserForDetailedDto.
        /// </remarks>
        /// <param name="id">Id of the user to be retrieved.</param>
        /// <response code="200">OK. Returns the spceficied user.</response>        
        /// <response code="401">Unauthorized. The provided JWT Token is wrong or it was not provided</response>              
        /// <response code="404">NotFound. The user with the specified id was not found.</response>        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await this.repo.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            var userToReturnToClient = this.mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturnToClient);
        }
    }
}