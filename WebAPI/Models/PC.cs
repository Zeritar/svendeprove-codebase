namespace SystemOverseer_API.Models
{
    public class PC
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsNetworked { get; set; }
        public uint IpAddress { get; set; }
        public byte[] MacAddress { get; set; } = new byte[6];
        public byte[] Pins { get; set; } = new byte[2];
        public bool IsOnline { get; set; }
    }
}
