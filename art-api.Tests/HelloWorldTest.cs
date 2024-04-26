namespace art_api.Tests;

using ArtsieApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

internal class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DatabaseFixture _databaseFixture;

    public CustomWebApplicationFactory(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture ?? throw new ArgumentNullException(nameof(databaseFixture));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IOptions<ArtsieDatabaseSettings>));

            // Access the MongoDbContainer from DatabaseFixture
            var container = _databaseFixture._container;

            // Create a new instance of ArtsieDatabaseSettings with test connection string
            var testConnectionString = container.GetConnectionString();
            var testArtsieSettings = new ArtsieDatabaseSettings
            {
                ConnectionString = testConnectionString,
                DatabaseName = "artsie-test",
                ArtCollectionName = "art",
                CommentsCollectionName = "comments",
                UsersCollectionName = "users"
            };

            // Register the new instance of IOptions<ArtsieDatabaseSettings>
            services.AddSingleton<IOptions<ArtsieDatabaseSettings>>(_ => Options.Create(testArtsieSettings));
        });
    }
}

public class DatabaseFixture : IDisposable
{
    private readonly IMongoDatabase _database;
    public MongoDbContainer _container { get; private set; }

    public DatabaseFixture()
    {

        Environment.SetEnvironmentVariable("TEST_ENVIRONMENT", "true");

        // Initialize Testcontainers.MongoDb MongoDB container
        _container = new MongoDbBuilder().Build();

        // Start the container
        _container.StartAsync().Wait();

        // Get MongoDB connection string
        var connectionString = _container.GetConnectionString();


        // Connect to the MongoDB database
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("artsie-test");

        // Seed initial data
        SeedTestData();
    }
    private void SeedTestData()
    {

        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string dataFolderPath = Path.Combine(currentDirectory, "data");

        string artFilePath = Path.Combine(dataFolderPath, "art.json");
        string commentsFilePath = Path.Combine(dataFolderPath, "comments.json");
        string usersFilePath = Path.Combine(dataFolderPath, "users.json");

        // Read JSON file containing test data
        string artJson = File.ReadAllText(artFilePath);
        string commentsJson = File.ReadAllText(commentsFilePath);
        string usersJson = File.ReadAllText(usersFilePath);

        // Deserialize JSON into list of objects
        var artData = JsonConvert.DeserializeObject<List<Art>>(artJson);
        var commentsData = JsonConvert.DeserializeObject<List<Comment>>(commentsJson);
        var usersData = JsonConvert.DeserializeObject<List<User>>(usersJson);
        // Implement seeding logic to populate the database with test data
        // For example:
        var artCollection = _database.GetCollection<Art>("art");
        var commentsCollection = _database.GetCollection<Comment>("comments");
        var usersCollection = _database.GetCollection<User>("users");

        artCollection.InsertMany(artData);
        commentsCollection.InsertMany(commentsData);
        usersCollection.InsertMany(usersData);

    }

    public void Dispose()
    {
        // Clean up after tests
        // Drop collections or perform any necessary cleanup operations
        // For example:
        _database.DropCollection("art");
        _database.DropCollection("comments");
        _database.DropCollection("users");

        // Dispose of the container
        _container.DisposeAsync().GetAwaiter().GetResult();

        Environment.SetEnvironmentVariable("TEST_ENVIRONMENT", null);

    }
}

public class Endpoints : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public Endpoints(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact(DisplayName = "200: GET /")]
    public async Task TestRootEndpoint()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal("Hello World!", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    [Fact(DisplayName = "404: GET /{*unknown}")]
    public async Task GetUnknownEndpoints_404()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var response = await client.GetAsync("/unknown");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact(DisplayName = "200: GET /api/art")]
    public async Task GetArt_200()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var response = await client.GetAsync("/api/art");
        List<Art>? content = JsonConvert.DeserializeObject<List<Art>>(await response.Content.ReadAsStringAsync());

        var expectedContent = new[]{
        new Art{
            Id="662289855f4d4b2786b31215",
            Name = "Cleopatra decorating the Tomb of Mark Anthony",
            Artist = "Angelica Kauffman",
            Description = "The scene portrays Cleopatra in a moment of mourning and love, as she decorates the tomb of Mark Antony, her lover and ally, who had committed suicide after being defeated by Octavian (later Emperor Augustus) in the Battle of Actium in 31 BC. Cleopatra, with a look of sorrow and determination, is shown placing a wreath on Antony's tomb, surrounded by mourners and attendants.",
        },
        new Art{
            Id="66261febd76faf52492be9da",
            Name = "Wanderer above the Sea of Fog",
            Artist = "Caspar David Friedrich",
            Description = "The painting depicts a solitary figure standing atop a rocky precipice, gazing out over a vast, misty landscape of mountains and fog-covered valleys. The figure, often interpreted as a representation of the Romantic wanderer or the sublime individual, stands with his back to the viewer, his face obscured, and his identity ambiguous. He is dressed in a dark overcoat and wears a wide-brimmed hat, adding to the sense of mystery and anonymity. The painting is often interpreted as a meditation on the human experience of the sublime—the overwhelming sense of awe, terror, and transcendence inspired by nature's grandeur.",
        },
        new Art{
            Id="66262026d76faf52492be9db",
            Name = "The Great Day of His Wrath",
            Artist = "John Martin",
            Description = "In this painting, Martin depicts a cataclysmic scene of divine judgment and apocalyptic destruction. The painting shows a vast landscape engulfed in chaos and devastation, with fire, brimstone, and volcanic eruptions wreaking havoc upon the earth. In the foreground, terrified figures, including women, men, and children, flee in panic from the impending doom, while others kneel in prayer or despair. The sky is darkened by swirling clouds, lightning bolts, and ominous storm clouds, heightening the sense of dread and impending doom.",

        }};

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedContent, content);


    }
    [Fact(DisplayName = "200: GET /api/art/{id}")]
    public async Task GetArtById_200()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var response = await client.GetAsync("/api/art/662289855f4d4b2786b31215");
        var content = JsonConvert.DeserializeObject<Art>(await response.Content.ReadAsStringAsync());

        var expectedContent = new
        {
            Name = "Cleopatra decorating the Tomb of Mark Anthony",
            Artist = "Angelica Kauffman",
            Description = "The scene portrays Cleopatra in a moment of mourning and love, as she decorates the tomb of Mark Antony, her lover and ally, who had committed suicide after being defeated by Octavian (later Emperor Augustus) in the Battle of Actium in 31 BC. Cleopatra, with a look of sorrow and determination, is shown placing a wreath on Antony's tomb, surrounded by mourners and attendants.",
        };

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedContent.Name, content.Name);
        Assert.Equal(expectedContent.Artist, content.Artist);
        Assert.Equal(expectedContent.Description, content.Description);
        Assert.IsType<string>(content.Id);
    }
    [Fact(DisplayName = "200: GET /art/{id}/comments")]
    public async Task GetCommentsByArtId_200()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var response = await client.GetAsync("/api/art/662289855f4d4b2786b31215/comments");
        List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());

        var expectedContent = new[]{
      new Comment{ Id="662620f4d76faf52492be9de", ArtId="662289855f4d4b2786b31215", Author="froggie", Body="Nice art!", Likes = 0 }};

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedContent, content);
    }
    [Fact(DisplayName = "201: POST api/art/{id}/comments")]
    public async Task PostComment_201()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var update = new
        {
            Author = "froggie",
            Body = "Neato!",
        };

        var expectedOutput = new[]{
        new {
            ArtId = "66261febd76faf52492be9da",
            Author = "froggie",
            Body = "Neato!",
            Likes = 0,
        }
        };

        var json = JsonConvert.SerializeObject(update);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/art/66261febd76faf52492be9da/comments", data);
        var comments = await client.GetAsync("/api/art/66261febd76faf52492be9da/comments");

        List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await comments.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(content[0].ArtId, expectedOutput[0].ArtId);
        Assert.Equal(content[0].Author, expectedOutput[0].Author);
        Assert.Equal(content[0].Body, expectedOutput[0].Body);
        Assert.Equal(content[0].Likes, expectedOutput[0].Likes);
        Assert.Single(content);
        Assert.IsType<string>(content[0].Id);
    }
    [Fact(DisplayName = "200: GET /api/comments")]
    public async Task GetComments_200()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var expectedContent = new[]{
        new Comment{
            Id = "662620f4d76faf52492be9de",
            Author="froggie",
            ArtId="662289855f4d4b2786b31215",
            Body="Nice art!",
            Likes=0,
        },
        new Comment{
            Id = "662621fed76faf52492be9e0",
            Author="froggie",
            ArtId="66262026d76faf52492be9db",
            Body="Cool!",
            Likes=0,
        }
        };

        var response = await client.GetAsync("/api/comments");
        List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedContent, content);
    }
    [Fact(DisplayName = "200: GET /api/comments/{id}")]
    public async Task GetCommentById_200()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var expectedContent = new
        {
            Id = "662621fed76faf52492be9e0",
            Author = "froggie",
            ArtId = "66262026d76faf52492be9db",
            Body = "Cool!",
            Likes = 0,
        };

        var response = await client.GetAsync("/api/comments/662621fed76faf52492be9e0");
        var content = JsonConvert.DeserializeObject<Comment>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedContent, content);
    }
    [Fact(DisplayName = "200: GET /api/users")]
    public async Task GetUsers_200()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var expectedContent = new[]{
        new {
            Id = "6626224bd76faf52492be9e1",
            Username="froggie"
        }
        };

        var response = await client.GetAsync("/api/users");
        List<User>? content = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedContent, content);
    }
    [Fact(DisplayName = "200: GET /api/users/{username}")]
    public async Task GetUserById_200()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var expectedContent = new
        {
            Id = "6626224bd76faf52492be9e1",
            Username = "froggie"
        };

        var response = await client.GetAsync("/api/users/froggie");
        var content = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(expectedContent, content);
    }
    [Fact(DisplayName = "201: POST api/users")]
    public async Task PostUser_201()
    {
        await using var application = new CustomWebApplicationFactory(_fixture);
        using var client = application.CreateClient();

        var update = new
        {
            Username = "duckie",
        };

        var json = JsonConvert.SerializeObject(update);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/users", data);
        var users = await client.GetAsync("/api/users/duckie");

        var content = JsonConvert.DeserializeObject<User>(await users.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(content.Username, update.Username);
        Assert.IsType<string>(content.Id);
    }
}
public class ArtEndpoint
{

    // [Fact(DisplayName = "400: GET /art/{id}")]

    // public async Task GetArtById_400()
    // {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.GetAsync("api/art/notanid");

    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    // }

    // [Fact(DisplayName = "404: GET /art/{id}")]
    // public async Task GetArtById_404()
    // {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.GetAsync("api/art/999999");

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }

    // [Fact(DisplayName = "400: GET /art/{id}/comments")]
    // public async Task GetCommentsByArtId_400()
    // {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.GetAsync("api/art/notanid/comments");

    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    // }
    // [Fact(DisplayName = "404: GET /art/{id}/comments")]
    // public async Task GetCommentsByArtId_404()
    // {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.GetAsync("api/art/999999/comments");

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }


    // [Fact(DisplayName = "404: PUT /art/{id}/comments")]
    // public async Task PostComment_404()
    // {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var update = new Comment
    //     {
    //         Author = "froggie",
    //         Body = "Neato!",
    //     };

    //     var json = JsonConvert.SerializeObject(update);
    //     var data = new StringContent(json, Encoding.UTF32, "application/json");

    //     var response = await client.PostAsync("/art/99999/comments", data);

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
    //   [Fact(DisplayName = "201: PUT /comments/{id}")]
    //   public async Task UpdateCommentLikes_201()
    //   {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var update = new Comment
    //     {
    //       Author = "froggie",
    //       Body = "Nice art!",
    //       Likes = 1
    //     };

    //     var json = JsonConvert.SerializeObject(update);
    //     var data = new StringContent(json, Encoding.UTF32, "application/json");

    //     var response = await client.PutAsync("/comments/2", data);
    //     var comments = await client.GetAsync("/art/3/comments");

    //     Comment? result = JsonConvert.DeserializeObject<Comment>(await response.Content.ReadAsStringAsync());
    //     List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await comments.Content.ReadAsStringAsync());

    //     Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    //     Assert.Equivalent(update.Likes, result.Likes);
    //     Assert.Equivalent(update.Likes, content[0].Likes);
    //   }

    //   [Fact(DisplayName = "404: PUT /comments/{id}")]
    //   public async Task UpdateCommentLikesById_404()
    //   {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();


    //     var update = new Comment
    //     {
    //       Author = "froggie",
    //       Body = "Nice art!",
    //       Likes = 1
    //     };

    //     var json = JsonConvert.SerializeObject(update);
    //     var data = new StringContent(json, Encoding.UTF32, "application/json");

    //     var response = await client.PutAsync("/comments/9999", data);

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    //   }
}

public class CommentsEndpoint
{

    //   [Fact(DisplayName = "204: DELETE /comments/{id}")]
    //   public async Task DeleteCommentById_204()
    //   {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.DeleteAsync("/comments/1");
    //     var comments = await client.GetAsync("/art/1/comments");

    //     List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await comments.Content.ReadAsStringAsync());

    //     Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    //     Assert.Empty(content);
    //   }
    //   [Fact(DisplayName = "400: DELETE /comments/{id}")]

    //   public async Task DeleteCommentById_400()
    //   {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.DeleteAsync("/comments/notanid");

    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //   }

    //   [Fact(DisplayName = "404: DELETE /comments/{id}")]
    //   public async Task DeleteCommentById_404()
    //   {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.DeleteAsync("/comments/999999");

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    //   }

}

public class UsersEndpoint
{

    //   [Fact(DisplayName = "400: GET /users/{id}")]

    //   public async Task GetUserById_400()
    //   {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.GetAsync("/users/notanid");

    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    //   }

    //   [Fact(DisplayName = "404: GET /users/{id}")]
    //   public async Task GetArtById_404()
    //   {
    //     await using var application = new WebApplicationFactory<Program>();
    //     using var client = application.CreateClient();

    //     var response = await client.GetAsync("/users/999999");

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    //   }

}

