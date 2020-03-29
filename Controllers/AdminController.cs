using System.Linq;
using System.Threading.Tasks;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyClean.API.Controllers
{
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

        [Authorize(Policy = "RequireBusinessOwnerRole")]
        [HttpGet("usersWithRoles")]
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


            return Ok(userList);
        }

        [Authorize(Policy = "RequireBusinessOwnerRole")]
        [HttpPost("editRoles/{userId}")]
        public async Task<IActionResult> EditRoles(string userId, RoleEditDto roleEditDto)
        {
            // Find the user in the DB and retrieve its actual roles
            var user = await userManager.FindByIdAsync(userId);
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