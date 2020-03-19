using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
    [Authorize]
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

        [HttpGet]
        public async Task<IActionResult> GetMachineUsages()
        {
            var machineUsages = await this.repo.GetMachineUsages();
            var machineUsagesToReturnToClient = this.mapper.Map<IEnumerable<MachineUsageForDetailedDto>>(machineUsages);
            return Ok(machineUsagesToReturnToClient);
        }
    }
}