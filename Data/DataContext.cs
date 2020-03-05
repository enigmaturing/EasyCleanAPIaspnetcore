using EasyClean.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyClean.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) { }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }  
        public DbSet<Purchase> Purchases { get; set; }
    }
}