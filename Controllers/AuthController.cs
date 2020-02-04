using EasyClean.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        
        public AuthController(IAuthRepository repo)
        {
            this.repo = repo;
        }
    }
}