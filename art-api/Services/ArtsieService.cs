using ArtsieApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ArtsieApi.Services;

public class ArtsieService
{
    private readonly IMongoCollection<Art> _artCollection;

    public ArtsieService(
        IOptions<ArtsieDatabaseSettings> artsieDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            artsieDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            artsieDatabaseSettings.Value.DatabaseName);

        _artCollection = mongoDatabase.GetCollection<Art>(
            artsieDatabaseSettings.Value.ArtCollectionName);
    }

    public async Task<List<Art>> GetAsync() =>
        await _artCollection.Find(_ => true).ToListAsync();

    public async Task<Art?> GetAsync(string id) =>
        await _artCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Art newArt) =>
        await _artCollection.InsertOneAsync(newArt);

    public async Task UpdateAsync(string id, Art updatedArt) =>
        await _artCollection.ReplaceOneAsync(x => x.Id == id, updatedArt);

    public async Task RemoveAsync(string id) =>
        await _artCollection.DeleteOneAsync(x => x.Id == id);
}