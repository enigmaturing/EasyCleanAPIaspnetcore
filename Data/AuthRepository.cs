using System;
using System.Threading.Tasks;
using EasyClean.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyClean.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext dataContext;

        public AuthRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /*
            The method Login checks if a given email is present in the table Users of our DB and then if
            the password given by the user is the same that is stored as a passowrdHash in the DB.
            If both conditions are true, a proper instance of the class User is return. If not, a
            value of null is returned
        */
        public async Task<User> Login(string email, string password)
        {
            var user = await this.dataContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return null;
            
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }


        /*
             The method VerifyPasswordHash first computes the hash for the password of the user (given a
             known passowrdSalt) and then compares it against the passwordHash stored in the DB.
             If both are the same, this method returns true. Otherwise a value of false is returned.
             NOTE that passwordHash is a aray of bytes, so we have to go through the array with a loop in
            order to compare it
         */
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // ATTENTION: In this case, the passwordSalt is known, so we pass it to HMACSHA512 so hash is
            // generated with that very salt
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                } 
            }
            return true;
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

        /*
            The method UserExists returns true if a given email is present in the table Users of our DB.
            Otherwise it returns a value of false
        */
        public async Task<bool> UserExists(string email)
        {
            if (await this.dataContext.Users.AnyAsync(x => x.Email == email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}