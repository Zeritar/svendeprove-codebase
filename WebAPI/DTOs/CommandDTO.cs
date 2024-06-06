namespace SystemOverseer_API.DTOs
{
    public class PostDTO
    {
        public string Command { get; set; } = "";
    }

    public class CommandDTO : PostDTO
    {
        
        public PCRequest Body {get; set; } = new PCRequest();
    }

    public class UpdateDTO : PostDTO
    {
        public string[] Body { get; set; } = new string[8];
    }
}