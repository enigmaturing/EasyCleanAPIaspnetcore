using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EasyClean.API.Controllers
{
    [OpenApiTag("Fallback",
              Description = "Controller with view support to fallback to " +
        "                   and render angular SPA")]
    [AllowAnonymous]
    public class Fallback : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                                            "wwwroot", "index.html"), "text/HTML");
        } 
    }
}