using Microsoft.EntityFrameworkCore;
using SystemOverseer_API.Models;

namespace SystemOverseer_API.Repositories
{
    public interface IPCRepository
    {
        Task<List<PC>> GetAll();
        Task<List<PC>> GetAllForUser(string username);
        Task<List<int>> GetIDs();

        Task<List<int>> GetIDsForUser(string username);
        Task<PC?> AddPC(PC pc);
        Task<PC?> GetPC(int id);
        Task<PC?> UpdatePC(PC pc);
        Task<PC?> DeletePC(int id);
    }

    public class PCRepository : IPCRepository
    {
        private readonly PCContext _context;
        private readonly UserContext _userContext;

        public PCRepository(PCContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<List<PC>> GetAll()
        {
            return await _context.PCs.ToListAsync();
        }

        public async Task<List<PC>> GetAllForUser(string username)
        {
            ApplicationUser? user = await _userContext.Users.FirstOrDefaultAsync(e => e.UserName == username);
            if (user == null) return new List<PC>();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return await _context.PCUsers.Where(e => e.UserId == user.Id).Include(e => e.PC).Select(e => e.PC).ToListAsync();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        public async Task<List<int>> GetIDs()
        {
            return await _context.PCs.Select(e => e.Id).ToListAsync();
        }

        public async Task<List<int>> GetIDsForUser(string username)
        {
            ApplicationUser? user = await _userContext.Users.FirstOrDefaultAsync(e => e.UserName == username);
            if (user == null) return new List<int>();
            return await _context.PCUsers.Where(e => e.UserId == user.Id).Select(e => e.PCId).ToListAsync();
        }

        public async Task<PC?> AddPC(PC pc)
        {
            _context.PCs.Add(pc);
            try
            {
                return await _context.SaveChangesAsync() > 0 ? pc : null;
            }
            // Throws ArgumentException if the PC already exists
            catch (ArgumentException)
            {
                return null;
            }
        }

        public async Task<PC?> GetPC(int id)
        {
            return await _context.PCs.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<PC?> UpdatePC(PC pc)
        {
            PC? _pc = GetPC(pc.Id).Result;
            if (_pc == null) return null;

            _pc.Name = pc.Name;
            _pc.IsNetworked = pc.IsNetworked;
            _pc.IpAddress = pc.IpAddress;
            _pc.MacAddress = pc.MacAddress;
            _pc.Pins = pc.Pins;
            _pc.IsOnline = pc.IsOnline;

            _context.PCs.Update(_pc);
            return await _context.SaveChangesAsync() > 0 ? pc : null;
        }

        public async Task<PC?> DeletePC(int id)
        {
            PC? pc = GetPC(id).Result;
            if (pc == null) return null;

            _context.PCs.Remove(pc);
            return await _context.SaveChangesAsync() > 0 ? pc : null;
        }
    }
}
