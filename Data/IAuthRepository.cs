using System.Threading.Tasks;
using EasyClean.API.Dtos;
using EasyClean.API.Models;

namespace EasyClean.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(UserForRegisterDto userForRegisterDto);
         Task<string> Login(UserForLoginDto userForLoginDto);
    }
}