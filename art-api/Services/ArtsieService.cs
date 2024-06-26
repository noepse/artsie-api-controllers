using ArtsieApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ArtsieApi.Services;

public class ArtsieService
{
    private readonly IMongoCollection<Art> _artCollection;

    private readonly IMongoCollection<Comment> _commentsCollection;
    private readonly IMongoCollection<User> _usersCollection;

    public ArtsieService(
        IOptions<ArtsieDatabaseSettings> artsieDatabaseSettings)
    {
        var connectionString = artsieDatabaseSettings.Value.ConnectionString;

        if(Environment.GetEnvironmentVariable("PRODUCTION_ENVIRONMENT") == "true"){
            connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
        }

        var mongoClient = new MongoClient(connectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            artsieDatabaseSettings.Value.DatabaseName);

        _artCollection = mongoDatabase.GetCollection<Art>(
            artsieDatabaseSettings.Value.ArtCollectionName);
        _commentsCollection = mongoDatabase.GetCollection<Comment>(
            artsieDatabaseSettings.Value.CommentsCollectionName);
        _usersCollection = mongoDatabase.GetCollection<User>(
            artsieDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<Art>> GetArtAsync() =>
        await _artCollection.Find(_ => true).ToListAsync();

    public async Task<Art?> GetArtAsync(string id) =>
        await _artCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    // public async Task CreateArtAsync(Art newArt) =>
    //     await _artCollection.InsertOneAsync(newArt);

    // public async Task UpdateArtAsync(string id, Art updatedArt) =>
    //     await _artCollection.ReplaceOneAsync(x => x.Id == id, updatedArt);

    // public async Task RemoveArtAsync(string id) =>
    //     await _artCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<List<Comment>> GetCommentsAsync() =>
await _commentsCollection.Find(_ => true).ToListAsync();

    public async Task<List<Comment>> GetCommentsOnArtAsync(string id) =>
await _commentsCollection.Find(x => x.ArtId == id).ToListAsync();

    public async Task<Comment?> GetCommentAsync(string id) =>
        await _commentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateCommentAsync(Comment newComment, string id) {
        newComment.ArtId = id;
        newComment.Likes = 0;
        await _commentsCollection.InsertOneAsync(newComment);
    }

    public async Task UpdateCommentAsync(string id, Comment updatedComment) =>
        await _commentsCollection.ReplaceOneAsync(x => x.Id == id, updatedComment);

    public async Task RemoveCommentAsync(string id) =>
        await _commentsCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<List<User>> GetUsersAsync() =>
await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetUserAsync(string username) =>
        await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    public async Task CreateUserAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task UpdateUserAsync(string username, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Username == username, updatedUser);

    public async Task RemoveUserAsync(string username) =>
        await _usersCollection.DeleteOneAsync(x => x.Username == username);
}