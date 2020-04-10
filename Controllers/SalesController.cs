using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EasyClean.API.Controllers
{
    [OpenApiTag("Sales", Description = "Handles usages of machines")]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IEasyCleanRepository repo;
        private readonly IMapper mapper;

        public SalesController(IEasyCleanRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        // GET: api/Sales
        /// <summary>
        /// Returns all usages registered in all machines.
        /// </summary>
        /// <response code="201">OK.</response>        
        /// <response code="400">It was not possible to retrieve any machine usage.</response>
        [HttpGet]
        public async Task<IActionResult> GetMachineUsages()
        {
            var machineUsages = await this.repo.GetMachineUsages();
            if (machineUsages == null)
            {
                return NotFound("Not possible to return any machine usage");
            }
            var machineUsagesToReturnToClient = this.mapper.Map<IEnumerable<MachineUsageForDetailedDto>>(machineUsages);
            return Ok(machineUsagesToReturnToClient);
        }
    }
}