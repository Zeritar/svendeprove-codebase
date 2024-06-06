using SystemOverseer_API.Models;

namespace SystemOverseer_API.DTOs
{
    public class PCRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsNetworked { get; set; }
        public uint IpAddress { get; set; }
        public int[] MacAddress { get; set; } = new int[6];
        public int[] Pins { get; set; } = new int[2];
    }

    public class PCDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsNetworked { get; set; }
        public uint IpAddress { get; set; }
        public uint[] MacAddress { get; set; } = new uint[6];
        public uint[] Pins { get; set; } = new uint[2];
        public bool IsOnline { get; set; }
    }

    public class PCSimpleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsOnline { get; set; }
        public int EntryID { get; set; }
    }

    public static class PCDTOExtensions
    {
        public static PCDTO ToDTO(this PC pc)
        {
            return new PCDTO
            {
                Id = pc.Id,
                Name = pc.Name,
                IsNetworked = pc.IsNetworked,
                IpAddress = pc.IpAddress,
                MacAddress = pc.MacAddress.Select(x => (uint)x).ToArray(),
                Pins = pc.Pins.Select(x => (uint)x).ToArray(),
                IsOnline = pc.IsOnline
            };
        }

        public static PCSimpleDTO ToSimpleDTO(this PC pc)
        {
            return new PCSimpleDTO
            {
                Id = pc.Id,
                Name = pc.Name,
                IsOnline = pc.IsOnline
            };
        }

        public static PCSimpleDTO SetEntryID(this PCSimpleDTO pcDto, int entryId)
        {
            pcDto.EntryID = entryId;
            return pcDto;
        }
    }

    public static class PCExtensions
    {
        public static PC ToPC(this PCDTO pc)
        {
            return new PC
            {
                Id = pc.Id,
                Name = pc.Name,
                IsNetworked = pc.IsNetworked,
                IpAddress = pc.IpAddress,
                MacAddress = pc.MacAddress.Select(x => (byte)x).ToArray(),
                Pins = pc.Pins.Select(x => (byte)x).ToArray(),
                IsOnline = pc.IsOnline
            };
        }
    }
}
