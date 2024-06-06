using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SystemOverseer_API.DTOs;
using SystemOverseer_API.Models;
using SystemOverseer_API.Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SystemOverseer_API.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SystemOverseer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PCController : ControllerBase
    {
        private readonly IPCService _pcService;

        public PCController(IPCService pcService)
        {
            _pcService = pcService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("getIDs")]
        public async Task<IActionResult> GetIDs()
        {
            List<int> ids = await _pcService.GetIDs();
            return Ok(ids);
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet]
        [Route("getIDsForUser")]
        public async Task<IActionResult> GetIDsForUser()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Substring(7));
            string username = jwt.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            List<int> ids = await _pcService.GetIDsForUser(username);
            return Ok(ids);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetAll()
        {
            List<PC> ids = await _pcService.GetAll();
            return Ok(ids.Select(e => e.ToDTO()));
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet]
        [Route("getAllForUser")]
        public async Task<IActionResult> GetAllForUser()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Substring(7));
            string username = jwt.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
            List<PC> pcs = await _pcService.GetAllForUser(username);
            return Ok(pcs.Select(e => e.ToDTO()));
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet]
        [Route("start/{id}")]
        public async Task<IActionResult> Start(int id)
        {
            PC? pc = _pcService.GetPC(id).Result;
            if (pc == null) return Ok(false);

            CommandDTO command = new CommandDTO()
            {
                Command = "start",
                Body = new PCRequest()
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    IsNetworked = pc.IsNetworked,
                    IpAddress = pc.IpAddress,
                    MacAddress = pc.MacAddress.Select(x => (int)x).ToArray(),
                    Pins = pc.Pins.Select(x => (int)x).ToArray()
                }
            };

            bool status = await _pcService.SendCommand(command);
            var json = JsonConvert.SerializeObject(command);
            return Ok(status);
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet]
        [Route("shutdown/{id}")]
        public async Task<IActionResult> Shutdown(int id)
        {
            PC? pc = _pcService.GetPC(id).Result;
            if (pc == null) return Ok(false);

            CommandDTO command = new CommandDTO()
            {
                Command = "shutdown",
                Body = new PCRequest()
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    IsNetworked = pc.IsNetworked,
                    IpAddress = pc.IpAddress,
                    MacAddress = pc.MacAddress.Select(x => (int)x).ToArray(),
                    Pins = pc.Pins.Select(x => (int)x).ToArray()
                }
            };

            bool status = await _pcService.SendCommand(command);
            var json = JsonConvert.SerializeObject(command);
            return Ok(status);
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet]
        [Route("heartbeat/{id}")]
        public async Task<IActionResult> Heartbeat(int id)
        {
            PC? pc = _pcService.GetPC(id).Result;
            if (pc == null) return Ok(false);

            CommandDTO command = new CommandDTO()
            {
                Command = "heartbeat",
                Body = new PCRequest()
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    IsNetworked = pc.IsNetworked,
                    IpAddress = pc.IpAddress,
                    MacAddress = pc.MacAddress.Select(x => (int)x).ToArray(),
                    Pins = pc.Pins.Select(x => (int)x).ToArray()
                }
            };

            bool status = await _pcService.SendCommand(command);

            if (status && pc.IsOnline == false)
            {
                pc.IsOnline = true;
                await _pcService.UpdatePC(pc);
            }
            else if (!status && pc.IsOnline == true)
            {
                pc.IsOnline = false;
                await _pcService.UpdatePC(pc);
            }

            var json = JsonConvert.SerializeObject(command);
            return Ok(status);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> Status()
        {
            List<PC> pcs = _pcService.GetAll().Result;
            string[] strings = new string[8];

            for (int i = 0; i < (pcs.Count < 4 ? pcs.Count : 4); i++)
            {

                strings[i * 2] = pcs[i].Name;
                strings[i * 2 + 1] = pcs[i].IsOnline ? "ONLINE" : "OFFLINE";
            }

            UpdateDTO update = new UpdateDTO()
            {
                Command = "update",
                Body = strings
            };

            await _pcService.SendCommand(update);
            var json = JsonConvert.SerializeObject(update);
            return Ok(json);
        }
    }
}
