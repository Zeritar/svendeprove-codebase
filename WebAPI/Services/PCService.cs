using SystemOverseer_API.Models;
using SystemOverseer_API.DTOs;
using System.Text;
using Newtonsoft.Json;
using SystemOverseer_API.Repositories;

namespace SystemOverseer_API.Services
{
    public interface IPCService
    {
        Task<List<PC>> GetAll();
        Task<List<PC>> GetAllForUser(string username);
        Task<List<int>> GetIDs();
        Task<List<int>> GetIDsForUser(string username);
        Task<bool> SendCommand(PostDTO command);
        Task<PC?> GetPC(int id);
        Task<PC?> AddPC(PC pc);
        Task<PC?> UpdatePC(PC pc);
        Task<PC?> DeletePC(int id);
    }


    public class PCService : IPCService
    {
        private readonly IPCRepository _pcRepository;

        public PCService(IPCRepository pcRepository)
        {
            _pcRepository = pcRepository;
        }

        public async Task<List<PC>> GetAll()
        {
            return await _pcRepository.GetAll();
        }

        public async Task<List<PC>> GetAllForUser(string username)
        {
            return await _pcRepository.GetAllForUser(username);
        }

        public async Task<List<int>> GetIDs()
        {
            return await _pcRepository.GetIDs();
        }

        public async Task<List<int>> GetIDsForUser(string username)
        {
            return await _pcRepository.GetIDsForUser(username);
        }

        public async Task<PC?> AddPC(PC pc)
        {
            return await _pcRepository.AddPC(pc);
        }

        public async Task<PC?> DeletePC(int id)
        {
            return await _pcRepository.DeletePC(id);
        }

        public async Task<PC?> GetPC(int id)
        {
            return await _pcRepository.GetPC(id);
        }

        public async Task<PC?> UpdatePC(PC pc)
        {
            return await _pcRepository.UpdatePC(pc);
        }

        public async Task<bool> SendCommand(PostDTO command)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            System.Diagnostics.Debug.WriteLine(json);

            try
            {
                var response = await httpClient.PostAsync("http://192.168.0.50/command", content);

                if (response.Content.ReadAsStringAsync().Result == "true")
                {
                    httpClient.Dispose();
                    return true;
                }
                else
                {
                    httpClient.Dispose();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                httpClient.Dispose();
                return false;
            }
        }
    }
}
