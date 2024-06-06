namespace SystemOverseer_API.DTOs
{
    public class WebSocketAppDTO
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public bool status { get; set; }
    }

    public class WebSocketESP32DTO
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
        public bool status { get; set; }
    }
}
