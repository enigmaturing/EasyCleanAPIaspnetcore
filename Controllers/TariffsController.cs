using System;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EasyClean.API.Controllers
{
    [OpenApiTag("Tariffs", Description = "Creates tariffs and administrate them.")]
    [Route("api/[controller]")]
    [ApiController]
    public class TariffsController : ControllerBase
    {
        private readonly IEasyCleanRepository repo;
        private readonly IMapper mapper;

        public TariffsController(IEasyCleanRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        // POST: api/Tariffs
        /// <summary>
        /// Creates a new tariff for a given machine group.
        /// (Requires roles: Admin or Developer)
        /// </summary>
        /// <param name="tariffForCreationDto">Details abot the tariff to be created</param>
        /// <response code="200">Ok.</response>
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>    
        /// <response code="404">It was not possible to create the tariff. No machine group found
        /// under the provided machine group id</response>
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateTariff(TariffForCreationDto tariffForCreationDto)
        {
            var tariff = mapper.Map<Tariff>(tariffForCreationDto);

            // Retrieve the object machineGroup this tariff belongs to.
            // Then store the retrieved object into the tariff
            var machineGroup = await repo.GetMachineGroup(tariff.MachineGroupId);
            if (machineGroup == null)
            {
                return NotFound("No machine group found under the provided id");
            }

            tariff.MachineGroup = machineGroup;
            repo.Add(tariff);

            if(await repo.SaveAll())
                return Ok(tariff);

            throw new Exception("Creating the tariff failed on save");
        }

        // GET: api/Tariffs
        /// <summary>
        /// Returns all tariffs for each machine group available.
        /// (Requires roles: FrontDeskEmployee, BackOfficeEmployee, Admin or Developer)
        /// </summary>
        /// <response code="200">Ok.</response> 
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>    
        /// <response code="404">No tariff found</response>
        [HttpGet]
        [Authorize(Policy = "RequireEmployeeRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTariffs()
        {
            var tariffs = await this.repo.GetTariffs();
            if (tariffs == null)
            {
                return NotFound("No tariff found");
            }
            else
            {
                return Ok(tariffs);
            }
        }

        // GET: api/Tariffs/{id}
        /// <summary>
        /// Retunrs information about a tariff by its id.
        /// (Requires roles: BackOfficeEmployee, Admin or Developer)
        /// </summary>
        /// <param name="id">Id of the tariff whose information should be retrieved</param>
        /// <response code="200">Ok.</response>  
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>    
        /// <response code="404">No tariff found under this tariff id</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireBackOfficeRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTariff(int id)
        {
            var tariffs = await this.repo.GetTariff(id);
            if (tariffs == null)
            {
                return NotFound("No tariff found");
            }
            else
            {
                return Ok(tariffs);
            }
        }

        // GET: api/Tariffs/machinegroup/{id}
        /// <summary>
        /// Retunrs all tariffs for a given machine group id.
        /// (Requires no specific roles. User must be just logged in)
        /// </summary>
        /// <param name="id">Id of the machine group whose tariffs should be retrieved.</param>
        /// <response code="200">Ok.</response>        
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>    
        /// <response code="404">No tariff found for this machine group.</response>
        [HttpGet("machinegroup/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTariffsOfMachineGroup(int id)
        {
            var tariffs = await this.repo.GetTariffsOfMachineGroup(id);
            if (tariffs == null)
            {
                return NotFound("No tariff found");
            }
            else
            {
                return Ok(tariffs);
            }
        }
    }
}