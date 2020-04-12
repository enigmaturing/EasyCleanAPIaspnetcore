using System.Collections.Generic;
using System.Threading.Tasks;
using EasyClean.API.Models;

namespace EasyClean.API.Data
{
    public interface IEasyCleanRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        void Update<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<IEnumerable<MachineGroup>> GetMachineGroups();
        Task<MachineGroup> GetMachineGroup(int id);
        Task<Machine> GetMachine(int id);
        Task<IEnumerable<MachineUsage>> GetMachineUsages();
        Task<IEnumerable<Tariff>> GetTariffs();
        Task<IEnumerable<Tariff>> GetTariffsOfMachineGroup(int id);
        Task<Tariff> GetTariff(int id);
        Task<IEnumerable<User>> GetClients();
    }
}
