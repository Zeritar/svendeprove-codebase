using Microsoft.EntityFrameworkCore;
using SystemOverseer_API.Models;

namespace SystemOverseer_API.Repositories
{
    public interface IPCUserRepository
    {
        Task<List<PCUser>> GetAllForUser(string userId);
        Task<List<PCUser>> GetAllForPC(int pcId);
        Task<PCUser?> AddPCUser(PCUser pcUser);
        Task<PCUser?> DeletePCUser(int id);
    }

    public class PCUserRepository : IPCUserRepository
    {
        private readonly PCContext _context;

        public PCUserRepository(PCContext context)
        {
            _context = context;
        }

        public async Task<List<PCUser>> GetAllForUser(string userId)
        {
            return await _context.PCUsers.Where(e => e.UserId == userId).ToListAsync();
        }

        public async Task<List<PCUser>> GetAllForPC(int pcId)
        {
            return await _context.PCUsers.Where(e => e.PCId == pcId).ToListAsync();
        }

        public async Task<PCUser?> AddPCUser(PCUser pcUser)
        {
            _context.PCUsers.Add(pcUser);

            return await _context.SaveChangesAsync() > 0 ? pcUser : null;

        }

        public async Task<PCUser?> DeletePCUser(int id)
        {
            PCUser? pcUser = await _context.PCUsers.FindAsync(id);
            
            if (pcUser == null) return null;
            _context.PCUsers.Remove(pcUser);

            return await _context.SaveChangesAsync() > 0 ? pcUser : null;
        }
    }
}
