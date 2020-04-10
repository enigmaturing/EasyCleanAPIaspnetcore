using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EasyClean.API.Controllers
{
    [OpenApiTag("Machines", Description = "Creates machines and machinegroups" +
        "                                  and administrate them")]
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

        // GET: api/Machines
        /// <summary>
        /// Returns all machine groups with information about them.
        /// </summary>
        /// <response code="201">OK.</response>        
        /// <response code="400">It was not possible to retrieve any machine group.</response>
        [HttpGet]
        public async Task<IActionResult> GetMachineGroups()
        {
            var machineGroups = await this.repo.GetMachineGroups();
            if (machineGroups == null)
            {
                return NotFound("Not possible to return any machine group");
            }
            var machineGroupsToReturnToClient = this.mapper.Map<IEnumerable<MachineGroupForListDto>>(machineGroups);
            return Ok(machineGroupsToReturnToClient);
        }
    }
}