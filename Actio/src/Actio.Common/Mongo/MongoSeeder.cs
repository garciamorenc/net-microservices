using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Actio.Common.Mongo
{
    public class MongoSeeder : IDatabaseSeeder
    {
        protected readonly IMongoDatabase _database;
        
        public MongoSeeder(IMongoDatabase database)
        {
            this._database = database;
        }

        public async Task SeedAsync()
        {
            var collectionCursor = await this._database.ListCollectionsAsync();
            var collections = await collectionCursor.ToListAsync();
            if (collections.Any()){
                return;
            }
            await CustomSeedAsync();
        }

        protected virtual async Task CustomSeedAsync(){
            await Task.CompletedTask;
        }
    }
}