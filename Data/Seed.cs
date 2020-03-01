using System.Collections.Generic;
using System.Linq;
using EasyClean.API.Models;
using Newtonsoft.Json;

namespace EasyClean.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext dataContext)
        {
            if (!dataContext.Users.Any())  // If there is no users stored in the DB, then seed dummy users from UserSeedData.json
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json"); // Read contents of the file
                var users = JsonConvert.DeserializeObject<List<User>>(userData);     // Deserialize it into a list of users
                foreach (var user in users)         // Store each of de deserialized user objects into de table Users of our DB
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Email = user.Email.ToLower();
                    dataContext.Users.Add(user);
                }
                // There is no need to save asynchsrounously because this method will only be called
                // once (when our application starts). So there is no way that there is more than one
                // user saving changes at the same time by the time this is called. Therefore, we dont
                // need an async call here
                dataContext.SaveChanges(); // Save sinchronouslly
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // Generate a HMAC to compute a hash from user's password
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
    }
}