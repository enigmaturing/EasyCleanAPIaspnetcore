using System.Threading.Tasks;
using EasyClean.API.Models;

namespace EasyClean.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string email, string password);
         Task<User> UserExists(string email);
    }
}