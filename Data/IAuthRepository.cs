using System.Threading.Tasks;
using EasyClean.API.Dtos;
using EasyClean.API.Models;

namespace EasyClean.API.Data
{
    public interface IAuthRepository
    {
        Task<User> RegisterClient(UserForRegisterClientDto userForRegisterClientDto);
        Task<User> RegisterEmployee(UserForRegisterEmployeeDto userForRegisterEmployeeDto);
        Task<string> Login(UserForLoginDto userForLoginDto);
    }
}