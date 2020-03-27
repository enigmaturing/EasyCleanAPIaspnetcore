using System.Linq;
using System.Threading.Tasks;
using EasyClean.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyClean.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext dataContext;
        public AdminController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [Authorize(Policy = "RequireBusinessOwnerRole")]
        [HttpGet]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            // Return a list of users along with their roles
            var userList = await dataContext.Users
                    .OrderBy(x => x.Email)
                    .Select(user => new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = (from userRole in user.UserRoles
                        join role in dataContext.Roles
                        on userRole.RoleId
                        equals role.Id
                        select role.Name).ToList()
                    })
                    .ToListAsync();


            return Ok(userList);
        }
    }
}