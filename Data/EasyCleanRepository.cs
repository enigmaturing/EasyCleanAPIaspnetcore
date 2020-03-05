using System.Collections.Generic;
using System.Threading.Tasks;
using EasyClean.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyClean.API.Data
{
    public class EasyCleanRepository : IEasyCleanRepository
    {
        private readonly DataContext dataContext;
        public EasyCleanRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public void Add<T>(T entity) where T : class
        {
            // We dont need to Add the entity asincronously because
            // because Add(entity) only adds it to memory but do not access
            // the DB. So there will be no simultaneous access to the DB
            this.dataContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            // We dont need to Remove the entity asincronously because
            // because Remove(entity) only removes it from memory but do not
            // access the DB. So there will be no simultaneous access to the DB
            this.dataContext.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            // IMPORTANT: We want to return also the photos of the user. The Photos
            // propierty is a navigation property and therefore we have to include
            // it specifically with Include(p => p.Photos)
            var user = await this.dataContext.Users.Include(p => p.Photos).Include(p => p.Purchases).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            // IMPORTANT: We want to return also the photos of the users. The Photos
            // propierty is a navigation property and therefore we have to include
            // it specifically with Include(p => p.Photos)
            var users = await this.dataContext.Users.Include(p => p.Photos).Include(p => p.Purchases).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            // Return true if saved items are greater than zero
            return await this.dataContext.SaveChangesAsync() > 0;
        }
    }
}