using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using SystemOverseer_API.DTOs;
using SystemOverseer_API.Models;
using SystemOverseer_API.Services;
using Newtonsoft.Json;

namespace SystemOverseer_API.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly IPCService _pcService;
        private readonly WebSocketService _ws;

        public WebSocketController(IPCService pcService, WebSocketService ws)
        {
            _pcService = pcService;
            _ws = ws;
        }

        [HttpGet("/ws/{id}/{command}")]
        public async Task Get(int id, string command)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                bool error = false;
                PC? pc = _pcService.GetPC(id).Result;

                if (pc == null)
                {
                    WebSocketAppDTO dto = new WebSocketAppDTO()
                    {
                        Id = id,
                        Message = command,
                        status = false
                    };
                    await Respond(webSocket, dto);
                    error = true;
                }

                bool responded = false;

                while (!responded && !error)
                {
                    string response = await _ws.ReceiveFromESP32();


                    if (response == $"{{{pc.Id},\"{command}\",\"Success\"}}")
                    {
                        responded = true;
                        pc.IsOnline = !pc.IsOnline;
                        await _pcService.UpdatePC(pc);

                        WebSocketAppDTO dto = new WebSocketAppDTO()
                        {
                            Id = pc.Id,
                            Message = command,
                            status = true
                        };
                        await Respond(webSocket, dto);

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

                    }
                    else if (response == $"{{{pc.Id},\"{command}\",\"Failed\"}}" || response == "Error")
                    {
                        responded = true;
                        WebSocketAppDTO dto = new WebSocketAppDTO()
                        {
                            Id = pc.Id,
                            Message = command,
                            status = false
                        };
                        await Respond(webSocket, dto);
                    }
                }

                // Close the WebSocket after the response has been sent.
                // If the WebSocket is left open, an exception will be thrown if the ESP32 loses connection.
                _ws.Close();
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task Respond(WebSocket webSocket, WebSocketAppDTO response)
        {
                var message = JsonConvert.SerializeObject(response);
                Console.WriteLine(message);
                var bytes = Encoding.UTF8.GetBytes(message);
                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
