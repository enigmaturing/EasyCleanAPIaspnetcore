using System;
using System.Threading.Tasks;
using AutoMapper;
using EasyClean.API.Data;
using EasyClean.API.Dtos;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyClean.API.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> GetTariffs()
        {
            var tariffs = await this.repo.GetTariffs();
            return Ok(tariffs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTariff(int id)
        {
            var tariffs = await this.repo.GetTariff(id);
            return Ok(tariffs);
        }

        [HttpGet("machinegroup/{id}")]
        public async Task<IActionResult> GetTariffsOfMachineGroup(int id)
        {
            var tariffs = await this.repo.GetTariffsOfMachineGroup(id);
            return Ok(tariffs);
        }
    }
}