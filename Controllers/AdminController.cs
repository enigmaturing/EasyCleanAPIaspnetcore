using System.Linq;
using System.Threading.Tasks;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;

namespace EasyClean.API.Controllers
{
    [OpenApiTag("Admin", Description = "Lets the admin administrate " +
                         " the roles of their employees.")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly UserManager<User> userManager;
        public AdminController(DataContext dataContext,
                              UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.dataContext = dataContext;
        }

        // GET: api/Admin/usersWithRoles
        /// <summary>
        /// Returns all users along with their roles.
        /// (Requires roles: Admin or Developer)
        /// </summary>
        /// <response code="200">OK. Returns the list of users with roles.</response>        
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>              
        /// <response code="404">NotFound. No users were found.</response>        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            // Return a list of users along with their roles
            var userList = await dataContext.Users
                    .OrderBy(x => x.Email)
                    .Select(user => new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Surname = user.Surname,
                        Roles = (from userRole in user.UserRoles
                                 join role in dataContext.Roles
                                 on userRole.RoleId
                                 equals role.Id
                                 select role.Name).ToList()
                    })
                    .ToListAsync();

            if (userList == null)
            {
                return NotFound();
            } 
            else 
            {
                return Ok(userList);
            }
        }

        // POST: api/Admin/editRoles/5
        /// <summary>
        /// Modifies the roles of a given user.
        /// (Requires roles: Admin or Developer)
        /// </summary>
        /// <remarks>
        /// The roles must be specified in the body of this post request in the form
        /// of the dto: roleEditDto
        /// </remarks>
        /// <param name="userId">Id of the user whose role must be modified.</param>
        /// <param name="roleEditDto">Roles to be modified</param>
        /// <response code="200">OK. Roles were edited. In addition, returns the specified roles.</response>        
        /// <response code="400">Bad request. Failed on dealing on adding /removing roles for user in DB.</response>
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>                
        /// <response code="404">NotFound. The user with the specified id was not found.</response>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditRoles(string userId, RoleEditDto roleEditDto)
        {
            // Find the user in the DB and retrieve its actual roles
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("No user found with that id");
            }
            var userRoles = await userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDto.RoleNames;
            // Because selectedRoles can be null, we ask if it is null
            // and in that case, we initialize it to an empty string
            selectedRoles = selectedRoles ?? new string[]{};

            // Add the roles that were included in the roleEditDto
            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to add the selected roles");

            // Remove the roles that were not included in the roleEditDto
            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded)
                return BadRequest("Failed to remove the unselected roles");

            // Return the active roles for the given user after modification
            // acording to roleEditDto
            return (Ok(await userManager.GetRolesAsync(user)));
        }
    }
}