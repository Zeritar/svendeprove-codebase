using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemOverseer_API.Auth;
using SystemOverseer_API.DTOs;
using SystemOverseer_API.Models;
using SystemOverseer_API.Services;

namespace SystemOverseer_API.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class PCAdminController : ControllerBase
    {
        private readonly IPCService _pcService;

        public PCAdminController(IPCService pcService)
        {
            _pcService = pcService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<PC> pcs = await _pcService.GetAll();
            return Ok(pcs);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            PC? pc = await _pcService.GetPC(id);
            return Ok(pc);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PCDTO pc)
        {
            PC pcToCreate = new PC()
            {
                Name = pc.Name,
                IsNetworked = pc.IsNetworked,
                IpAddress = pc.IpAddress,
                MacAddress = pc.MacAddress.Select(e => (byte)e).ToArray(),
                Pins = pc.Pins.Select(e => (byte)e).ToArray(),
                IsOnline = pc.IsOnline
            };

            PC? newPC = await _pcService.AddPC(pcToCreate);
            return Ok(newPC);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PCDTO pc)
        {
            PC pcToUpdate = new PC()
            {
                Id = pc.Id,
                Name = pc.Name,
                IsNetworked = pc.IsNetworked,
                IpAddress = pc.IpAddress,
                MacAddress = pc.MacAddress.Select(e => (byte)e).ToArray(),
                Pins = pc.Pins.Select(e => (byte)e).ToArray(),
                IsOnline = pc.IsOnline
            };

            PC? updatedPC = await _pcService.UpdatePC(pcToUpdate);
            return Ok(updatedPC);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            PC? deletedPC = await _pcService.DeletePC(id);
            return Ok(deletedPC);
        }

    }
}
