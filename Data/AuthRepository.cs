using System;
using System.Threading.Tasks;
using EasyClean.API.Models;

namespace EasyClean.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext dataContext;

        public AuthRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public Task<User> Login(string email, string password)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // await next two lines because we work with async methods (much better since we are dealing
            // with a DB and inserting a value in it can take unexpectedly long)
            await this.dataContext.Users.AddAsync(user);    // Inserts the user into the table of DB asynchronously
            await this.dataContext.SaveChangesAsync();      // IMPORTANT: Needed after AddAsync. Otherwise user wont be saved!

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Generate a HMAC to compute a hash from user's password
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public Task<User> UserExists(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}