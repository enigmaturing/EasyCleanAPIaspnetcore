using EasyClean.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyClean.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int,
                                                 IdentityUserClaim<int>, UserRole,
                                                 IdentityUserLogin<int>, 
                                                 IdentityRoleClaim<int>,
                                                 IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) { }
        public DbSet<MachineUsage> MachineUsages { get; set; }
        public DbSet<MachineGroup> MachineGroups { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Topup> Topups { get; set; }
        public DbSet<Machine> Machines { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(userRole => new {userRole.UserId, userRole.RoleId});

                userRole.HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleId)
                .IsRequired();

                userRole.HasOne(userRole => userRole.User)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.UserId)
                .IsRequired();
            });
        }
    }
}