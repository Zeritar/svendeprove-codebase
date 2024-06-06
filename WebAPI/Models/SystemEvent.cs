namespace SystemOverseer_API.Models
{
    public class SystemEvent
    {
        public int Id { get; set; }
        public int PCId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string EventType { get; set; } = "";
    }
}