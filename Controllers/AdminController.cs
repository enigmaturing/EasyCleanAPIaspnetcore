using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [Authorize(Policy = "RequireBusinessOwnerRole")]
        [HttpGet]
        public IActionResult GetUsersWithRoles()
        {
            return Ok("Only boss can access this");
        }
    }
}