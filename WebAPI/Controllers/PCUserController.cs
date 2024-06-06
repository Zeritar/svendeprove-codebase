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
    public class PCUserController : ControllerBase
    {
        private readonly IPCUserService _pcUserService;

        public PCUserController(IPCUserService pcUserService)
        {
            _pcUserService = pcUserService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersWithPCs()
        {
            var pcUsers = await _pcUserService.GetAllUsersWithPCs();
            return Ok(pcUsers);
        }

        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetAllForUser(string username)
        {
            var pcUsers = await _pcUserService.GetAllForUser(username);
            return Ok(pcUsers);
        }

        [HttpGet("pc/{pcId}")]
        public async Task<IActionResult> GetAllForPC(int pcId)
        {
            var pcUsers = await _pcUserService.GetAllForPC(pcId);
            return Ok(pcUsers);
        }

        [HttpPost]
        public async Task<IActionResult> AddPCToUser([FromBody] PCUserDTO pcUser)
        {
            var result = await _pcUserService.AddPCToUser(pcUser.PCId, pcUser.Username);
            if (result == null)
            {
                return BadRequest("Unable to add PC to user");
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePCUser(int id)
        {
            var result = await _pcUserService.DeletePCUser(id);
            if (result == null)
            {
                return NotFound("PCUser not found");
            }
            return Ok(result);
        }
    }
}
