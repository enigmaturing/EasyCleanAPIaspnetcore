using System.Collections.Generic;
using System.Linq;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace EasyClean.API.Data
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> userManager)
        {
            if (!userManager.Users.Any())  // If there is no users stored in the DB, then seed dummy users from UserSeedData.json
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json"); // Read contents of the file
                var users = JsonConvert.DeserializeObject<List<User>>(userData);     // Deserialize it into a list of users
                foreach (var user in users)         // Store each of the deserialized user-objects into de table Users of our DB
                {
                    // In the following line we make use of Wait() because that is an async method being called from a meetod
                    // (SeedUsers) that we dont want to defune as async, so we wait until the async Method CreateAsync is done. 
                    userManager.CreateAsync(user, "password").Wait();
                }
            }
        }

        public static void SeedMachineGroups(DataContext dataContext)
        {
            if (!dataContext.MachineGroups.Any()) 
            {
                var machineGroupData = System.IO.File.ReadAllText("Data/MachineGroupSeedData.json"); // Read contents of the file
                var machineGroups = JsonConvert.DeserializeObject<List<MachineGroup>>(machineGroupData);     // Deserialize it into a list of machineGroups
                foreach (var machineGroup in machineGroups)         // Store each of the deserialized machineGroup-objects into de table MachineGroups of our DB
                {
                    dataContext.MachineGroups.Add(machineGroup);
                }
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