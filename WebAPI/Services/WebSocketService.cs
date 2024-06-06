using System.Net.WebSockets;
using System.Text;

namespace SystemOverseer_API.Services
{
    public class WebSocketService : IDisposable
    {
        private readonly Uri _serverUri;
        ClientWebSocket wsESP32 = new ClientWebSocket();

        public WebSocketService()
        {
            string serverUri = "ws://192.168.0.50/ws";
            _serverUri = new Uri(serverUri);
            ConnectAsync().Wait();
        }

        private async Task ConnectAsync()
        {
            await wsESP32.ConnectAsync(_serverUri, CancellationToken.None);
        }

        // Currently we never send messages to the ESP32, but this method is here for future use.
        public async Task<bool> SendToESP32(string message)
        {
            if (wsESP32.State != WebSocketState.Open)
            {
                Console.WriteLine("WebSocket is not open");
                return false;
            }
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await wsESP32.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("Message sent: " + message);
            return true;
        }

        public async Task<string> ReceiveFromESP32()
        {
            try
            {
                if (wsESP32.State != WebSocketState.Open)
                {
                    await wsESP32.ConnectAsync(_serverUri, CancellationToken.None);
                    if (wsESP32.State != WebSocketState.Open)
                    {
                        Console.WriteLine("WebSocket is not open");
                        return "Error";
                    }
                }
                byte[] buffer = new byte[1024];
                WebSocketReceiveResult result = await wsESP32.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine("Message received: " + message);
                    return message;
                }

                Console.WriteLine("Error: Reached end of try block without getting a known message.");

                return "Error";
            }
            catch (Exception e)
            {
                if (wsESP32.State != WebSocketState.Closed)
                {
                    Close();
                }
                Console.WriteLine($"Error: Reached catch block with exception: {e.Message}");
                return "Error";
            }
        }

        // Cannot reuse a WebSocket after it has been closed. Dispose of it and create a new one.
        public void Close()
        {
            wsESP32.Dispose();
            wsESP32 = new ClientWebSocket();
        }

        // For DI to dispose of the WebSocket when the service is disposed.
        public void Dispose()
        {
            if (wsESP32.State != WebSocketState.Closed)
                wsESP32.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).Wait();

            wsESP32.Dispose();
        }
    }
}
