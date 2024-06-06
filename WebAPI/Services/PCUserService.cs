using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SystemOverseer_API.DTOs;
using SystemOverseer_API.Models;
using SystemOverseer_API.Repositories;

namespace SystemOverseer_API.Services
{
    public interface IPCUserService
    {
        Task<List<UserPCsDTO>> GetAllUsersWithPCs();
        Task<List<PCUser>> GetAllForUser(string userId);
        Task<List<PCUser>> GetAllForPC(int pcId);
        Task<PCUser?> AddPCToUser(int pcId, string username);
        Task<PCUser?> DeletePCUser(int id);
    }

    public class PCUserService : IPCUserService
    {
        private readonly IUserService _userService;
        private readonly IPCUserRepository _pcUserRepository;
        private readonly IPCService _pcService;

        public PCUserService(IUserService userService, IPCUserRepository pcUserRepository, IPCService pCService)
        {
            _userService = userService;
            _pcUserRepository = pcUserRepository;
            _pcService = pCService;
        }

        public async Task<List<UserPCsDTO>> GetAllUsersWithPCs()
        {
            var users = await _userService.GetAllUsers();

            if (users.Count == 0)
            {
                return new List<UserPCsDTO>();
            }

            List<UserPCsDTO> userPCs = new List<UserPCsDTO>();

            foreach (var user in users)
            {
                var pcUsers = await GetAllForUser(user);

                List<PCSimpleDTO> pcs = new List<PCSimpleDTO>();

                if (pcUsers.Count > 0)
                {
                    foreach (var pcUser in pcUsers)
                    {
                        var pc = await _pcService.GetPC(pcUser.PCId);

                        if (pc == null)
                        {
                            continue;
                        }

                        pcs.Add(pc.ToSimpleDTO().SetEntryID(pcUser.Id));
                    }
                }

                UserPCsDTO userPCsDTO = new UserPCsDTO { Username = user, PCs = pcs };

                userPCs.Add(userPCsDTO);
            }

            return userPCs;
        }

        public async Task<List<PCUser>> GetAllForUser(string username)
        {
            var user = await _userService.FindByNameAsync(username);

            if (user == null)
            {
                return new List<PCUser>();
            }

            return await _pcUserRepository.GetAllForUser(user.Id);
        }

        public async Task<List<PCUser>> GetAllForPC(int pcId)
        {
            return await _pcUserRepository.GetAllForPC(pcId);
        }

        public async Task<PCUser?> AddPCToUser(int pcId, string username)
        {
            var user = await _userService.FindByNameAsync(username);

            if (user == null)
            {
                return null;
            }

            PCUser pcUser = new PCUser { PCId = pcId, UserId = user.Id };
            return await _pcUserRepository.AddPCUser(pcUser);
        }

        public async Task<PCUser?> DeletePCUser(int id)
        {
            return await _pcUserRepository.DeletePCUser(id);
        }
    }
}
