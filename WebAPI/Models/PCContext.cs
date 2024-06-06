using Microsoft.EntityFrameworkCore;

namespace SystemOverseer_API.Models
{
    public class PCContext : DbContext
    {
        public PCContext(DbContextOptions<PCContext> options) : base(options)
        {
        }

        public DbSet<PC> PCs { get; set; }
        public DbSet<PCUser> PCUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PC>().HasData(
            new PC
            {
                Id = 1,
                Name = "HP Compaq",
                IsNetworked = false,
                IpAddress = 0,
                MacAddress = new byte[] { 0, 0, 0, 0, 0, 0 },
                Pins = new byte[2] { 27, 23 },
                IsOnline = false
            },
            new PC
            {
                Id = 2,
                Name = "Lenovo ThinkCentre",
                IsNetworked = false,
                IpAddress = 0,
                MacAddress = new byte[] { 0, 0, 0, 0, 0, 0 },
                Pins = new byte[2] { 25, 21 },
                IsOnline = false
            },
            new PC
            {
                Id = 3,
                Name = "Lenovo ThinkPad",
                IsNetworked = true,
                IpAddress = 0x6900A8C0,
                MacAddress = new byte[6] { 0xF8, 0x75, 0xA4, 0x01, 0x4F, 0x89 },
                Pins = new byte[2] { 0, 0 },
                IsOnline = false
            });
        }
    }
}
