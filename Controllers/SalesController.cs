using System;
using System.Collections.Generic;
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
    [OpenApiTag("Sales", Description = "Handles usages of machines.")]
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

        // GET: api/Sales/machineUsages
        /// <summary>
        /// Returns all usages registered in all machines.
        /// (Requires roles: Admin, BackofficeEmployee or Developer)
        /// </summary>
        /// <response code="200">OK.</response>       
        /// <response code="401">No BackOfficeEmployee, Admin or Developer role associated to this JWT Token.</response>
        /// <response code="404">No machine usages found.</response>
        [HttpGet("machineUsages")]
        [Authorize(Policy = "RequireBackOfficeRole")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMachineUsages()
        {
            var machineUsages = await this.repo.GetMachineUsages();
            if (machineUsages == null)
            {
                return NotFound("No machine usages found");
            }
            var machineUsagesToReturnToClient = this.mapper.Map<IEnumerable<MachineUsageForDetailedDto>>(machineUsages);
            return Ok(machineUsagesToReturnToClient);
        }

        // POST: api/Sales/machineUsages
        /// <summary>
        /// Creates a new machine usage of a given machine for a given user.
        /// (Requires role: Client)
        /// </summary>
        /// <param name="machineUsageForCreationDto">Details abot the machine usage to be created.</param>
        /// <response code="200">Ok.</response>        
        /// <response code="400">Client has not enough credit to make this usage in this machine</response>
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>    
        /// <response code="404">No user, machine or tariff found for the provided id.</response>
        [Authorize(Policy = "RequireClientRole")]
        [HttpPost("machineUsages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateMachineUsage(MachineUsageForCreationDto machineUsageForCreationDto)
        {
            var machineUsage= mapper.Map<MachineUsage>(machineUsageForCreationDto);

            var user = await repo.GetUser(machineUsage.UserId);
            var machine = await repo.GetMachine(machineUsage.MachineId);
            var tariff = await repo.GetTariff(machineUsage.TariffId);
            
            if (user == null || machine == null || tariff == null)
            {
                return NotFound("No user, machine or tariff found for the provided id");
            }

            var totalPrice = machineUsage.QuantityOfServicesBooked * tariff.Price;

            if (user.RemainingCredit < totalPrice)
            {
                return BadRequest("User has not enough credit to make this usage");
            }

            machineUsage.User = user;
            machineUsage.Machine = machine;
            machineUsage.Tariff = tariff;
            machineUsage.Date = DateTime.Now;

            repo.Add(machineUsage);
            
            user.RemainingCredit -= totalPrice;
            repo.Update(user);

            if (await repo.SaveAll())
            {
                return Ok(mapper.Map<UserForListDto>(user));
            }

            throw new Exception("Creating the machine usage failed on save");
        }

        // POST: api/Sales/topups
        /// <summary>
        /// Creates a new topup for a given user.
        /// (Requires roles: FrontDeskEmployee, Admin or Developer)
        /// </summary>
        /// <param name="topupForCreationDto">Details about the topup to be created.</param>
        /// <response code="200">Ok.</response>        
        /// <response code="401">Unauthorized. The provided JWT Token is wrong, 
        /// does not have the proper role or it was not provided.</response>    
        /// <response code="404">No client or employee found for the provided id.</response>
        [Authorize(Policy = "RequireClientManagementRole")]
        [HttpPost("topups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateTopup(TopupForCreationDto topupForCreationDto)
        {
            var topup = mapper.Map<Topup>(topupForCreationDto);

            var employee = await repo.GetUser(topupForCreationDto.EmployeeId);
            var client = await repo.GetUser(topupForCreationDto.UserId);

            if (employee == null || client == null)
            {
                return NotFound("No client or employee found for the provided id");
            }

            topup.User = client;
            topup.NameOfEmployee = employee.Name + " " + employee.Surname;
            topup.DateOfTopup = DateTime.Now;

            client.RemainingCredit += topupForCreationDto.Amount;

            repo.Add(topup);

            if (await repo.SaveAll())
            {
                return Ok(mapper.Map<TopupsForDetailedDto>(topup));
            }

            throw new Exception("Creating the topup failed on save");
        }

        // GET: api/Sales/topups/5
        /// <summary>
        /// Returns topups of a given client.
        /// (Requires roles: Admin, FrontDeskEmployee, BackofficeEmployee or Developer)
        /// </summary>
        /// <param name="id">Id of the client whose topups must be retrieved.</param>
        /// <response code="200">OK.</response>       
        /// <response code="401">No FrontDeskEmployee, BackOfficeEmployee, Admin or Developer role associated to this JWT Token.</response>
        /// <response code="404">No machine usages found.</response>
        [HttpGet("topups/{id}")]
        [Authorize(Policy = "RequireEmployeeRole")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopupsOfClient(int id)
        {
            var topups = await this.repo.GetTopupsOfClient(id);
            if (topups == null)
            {
                return NotFound("No topups found for this client");
            }
            var topupsToReturnToClient = this.mapper.Map<IEnumerable<TopupsForDetailedDto>>(topups);
            return Ok(topupsToReturnToClient);
        }
    }
}
