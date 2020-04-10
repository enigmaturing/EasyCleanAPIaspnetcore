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
        /// <response code="200">OK.</response>        
        /// <response code="404">No machine usages found.</response>
        [HttpGet]
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

        // POST: api/Sales
        /// <summary>
        /// Creates a new machine usage of a given machine for a given user.
        /// </summary>
        /// <param name="machineUsageForCreationDto">Details abot the machine usage to be created.</param>
        /// <response code="201">Created.</response>        
        /// <response code="404">No user, machine or tariff found for the provided id.</response>
        /// <response code="401">No client role associated to this JWT Token</response>
        /// <response code="400">Client has not enough credit to make this usage in this machine</response>
        [Authorize(Policy = "RequireClientRole")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                return Ok();
            }

            throw new Exception("Creating the machine usage failed on save");
        }
    }
}
