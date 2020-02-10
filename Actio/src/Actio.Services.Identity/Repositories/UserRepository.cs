using System;
using System.Threading.Tasks;
using Actio.Services.Identity.Domain.Models;
using Actio.Services.Identity.Domain.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Actio.Services.Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        public IMongoDatabase _database { get; }
        public UserRepository(IMongoDatabase database)
        {
            this._database = database;

        }

        public async Task<User> GetAsync(Guid id)
            => await this.Collection
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<User> GetAsync(string email)
            => await this.Collection
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Email == email.ToLowerInvariant());

        public async Task AddAsync(User user)
            => await this.Collection.InsertOneAsync(user);

        private IMongoCollection<User> Collection 
            => this._database.GetCollection<User>("Users");
    }
}