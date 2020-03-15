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
    public class MachinesController : ControllerBase
    {
        private readonly IEasyCleanRepository repo;
        private readonly IMapper mapper;

        public MachinesController(IEasyCleanRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetMachines()
        {
            var machines = await this.repo.GetMachines();
            var machinesToReturnToClient = this.mapper.Map<IEnumerable<MachineForListDto>>(machines);
            return Ok(machinesToReturnToClient);
        }

        // ToDo: Implement GetMachine(int id)
        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetMachine(int id)
        // {
        //     var machines = await this.repo.GetMachine(id);
        //     var machinesToReturnToClient = this.mapper.Map<MachineForDetailedDto>(machines);
        //     return Ok(machinesToReturnToClient);
        // }
    }
}