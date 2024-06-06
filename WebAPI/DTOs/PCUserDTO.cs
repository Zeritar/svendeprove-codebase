namespace SystemOverseer_API.DTOs
{
    public class PCUserDTO
    {
        public int PCId { get; set; }
        public string Username { get; set; } = "";
    }

    public class UserPCsDTO
    {
        public string Username { get; set; } = "";
        public List<PCSimpleDTO> PCs { get; set; } = new();
    }
}
