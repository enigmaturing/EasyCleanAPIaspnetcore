using System;
using EasyClean.API.Data;
using EasyClean.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyClean.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            // SEED DB WITH DUMMY USERS IF DB IS EMPTY
            // Get the dataContext to pass it as an argument to the SeedUsers method
            // in order to fill our DB with dummy data, if the tables are empty
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dataContext = services.GetRequiredService<DataContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    dataContext.Database.Migrate();  // Apply any pending migration and create DB if it does not exist
                    Seed.SeedUsers(userManager);
                    Seed.SeedMachineGroups(dataContext);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                }
            }
            // Once our DB is filled with data, run the application
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

