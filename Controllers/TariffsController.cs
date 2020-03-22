using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
    [Authorize]
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

        [HttpPost]
        public async Task<IActionResult> CreateTariff(TariffForCreationDto tariffForCreationDto)
        {
            var tariff = mapper.Map<Tariff>(tariffForCreationDto);

            // Retrieve the object machineGroup this tariff belongs to.
            // Then store the retrieved objeto into the tariff
            var machineGroup = await repo.GetMachineGroup(tariff.MachineGroupId);
            tariff.MachineGroup = machineGroup;

            repo.Add(tariff);

            if(await repo.SaveAll())
                return Ok();

            throw new Exception("Creating the tariff failed on save");
        }
    }
}