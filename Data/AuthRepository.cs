using System.Threading.Tasks;
using EasyClean.API.Models;

namespace EasyClean.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        public Task<User> Login(string email, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Register(User user, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> UserExists(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}