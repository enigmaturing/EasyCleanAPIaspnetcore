using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
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
        public async Task<IActionResult> GetMachineGroups()
        {
            var machineGroups = await this.repo.GetMachineGroups();
            var machineGroupsToReturnToClient = this.mapper.Map<IEnumerable<MachineGroupForListDto>>(machineGroups);
            return Ok(machineGroupsToReturnToClient);
        }
    }
}