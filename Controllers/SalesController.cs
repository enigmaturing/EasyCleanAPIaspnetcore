using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration config;

        public SalesController(IEasyCleanRepository repo, IMapper mapper, IConfiguration config)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.config = config;  // Inject IConfiguration from Startup.cs, so we can retrieve our particle token in method GetMachineUages()
        }

        // GET: api/Sales/machineUsages
        /// <summary>
        /// Returns all usages registered in all machines.
        /// (Requires roles: Admin, Backoffice or Developer)
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
            // map machineUsage from the DTO recieved with the http call
            var machineUsage= mapper.Map<MachineUsage>(machineUsageForCreationDto);

            // check if user, machine and tariff exist in DB
            var user = await repo.GetUser(machineUsage.UserId);
            var machine = await repo.GetMachine(machineUsage.MachineId);
            var tariff = await repo.GetTariff(machineUsage.TariffId);
            if (user == null || machine == null || tariff == null)
            {
                return NotFound("No user, machine or tariff found for the provided id");
            }

            // check if user has enough credit to make use of this machine
            var totalPrice = machineUsage.QuantityOfServicesBooked * tariff.Price;
            if (user.RemainingCredit < totalPrice)
            {
                return BadRequest("User has not enough credit to make this usage");
            }

            // Add machine usage to DB
            machineUsage.User = user;
            machineUsage.Machine = machine;
            machineUsage.Tariff = tariff;
            machineUsage.Date = DateTime.Now;
            repo.Add(machineUsage);
            
            // Update client's remaining credit in DB
            user.RemainingCredit -= totalPrice;
            repo.Update(user);

            // Activate the desired machine
            using (var httpClient = new HttpClient())
            {
                var formcontent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("arg", "on")} );
                var particleAccessToken = this.config.GetSection("AppSettings:ParticleAccessToken").Value;
                var request = await httpClient.PostAsync("https://api.particle.io/v1/devices/e00fce68ba9a1f5ea4870186/motorToggle?access_token=" + particleAccessToken, formcontent);
            }

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
    }
}
