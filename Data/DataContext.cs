using EasyClean.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyClean.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<MachineUsage> MachineUsages { get; set; }
        public DbSet<MachineGroup> MachineGroups { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Topup> Topups { get; set; }
        public DbSet<Machine> Machines { get; set; }
    }
}