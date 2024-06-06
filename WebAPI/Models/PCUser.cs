using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemOverseer_API.Models
{
    public class PCUser
    {
        public int Id { get; set; }

        [Required]
        public int PCId { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        [ForeignKey("PCId")]
        public virtual PC? PC { get; set; }
    }
}
