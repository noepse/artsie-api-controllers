namespace art_api.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using Artsie.DB;
using Newtonsoft.Json;
using System.Net;

public class HelloWorldTest
{
  [Fact]
  public async Task TestRootEndpoint()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/");
    var content = await response.Content.ReadAsStringAsync();

    Assert.Equal("Hello World!", content);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }
}
public class ArtEndpoint
{
  [Fact(DisplayName = "200: GET /art/{id}")]
  public async Task GetArtById_200()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/art/1");
    var content = JsonConvert.DeserializeObject<Art>(await response.Content.ReadAsStringAsync());

    var expectedContent = new Art
    {
      Id = 1,
      Name = "Cleopatra decorating the Tomb of Mark Anthony",
      Artist = "Angelica Kauffman",
      Description = "The scene portrays Cleopatra in a moment of mourning and love, as she decorates the tomb of Mark Antony, her lover and ally, who had committed suicide after being defeated by Octavian (later Emperor Augustus) in the Battle of Actium in 31 BC. Cleopatra, with a look of sorrow and determination, is shown placing a wreath on Antony's tomb, surrounded by mourners and attendants.",
    };

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equivalent(expectedContent, content);
  }
  [Fact(DisplayName = "400: GET /art/{id}")]

  public async Task GetArtById_400()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/art/notanid");

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact(DisplayName = "404: GET /art/{id}")]
  public async Task GetArtById_404()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/art/999999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
  [Fact(DisplayName = "200: GET /art/{id}/comments")]
  public async Task GetCommentsByArtId_200()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    // Reseed comments
    CommentsDB.Seed();

    var response = await client.GetAsync("/art/1/comments");
    List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());

    var expectedContent = new[]{
      new Comment{ Id=1, ArtId=1, Author="froggie", Body="Nice art!" }};

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equivalent(expectedContent, content);
  }
  [Fact(DisplayName = "400: GET /art/{id}/comments")]
  public async Task GetCommentsByArtId_400()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/art/notanid/comments");

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }
  [Fact(DisplayName = "404: GET /art/{id}/comments")]
  public async Task GetCommentsByArtId_404()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

// Reseed comments
    CommentsDB.Seed();

    var response = await client.GetAsync("/art/999999/comments");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}

public class CommentsEndpoint
{
  [Fact(DisplayName = "204: DELETE /comments/{id}")]
  public async Task DeleteCommentById_204()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    // Reseed comments
    CommentsDB.Seed();

    var response = await client.DeleteAsync("/comments/1");
    var comments = await client.GetAsync("/art/1/comments");

    List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await comments.Content.ReadAsStringAsync());

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    Assert.Empty(content);
  }
  [Fact(DisplayName = "400: DELETE /comments/{id}")]

  public async Task DeleteCommentById_400()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    // Reseed comments
    CommentsDB.Seed();

    var response = await client.DeleteAsync("/comments/notanid");

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact(DisplayName = "404: DELETE /comments/{id}")]
  public async Task GetArtById_404()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    // Reseed comments
    CommentsDB.Seed();

    var response = await client.DeleteAsync("/comments/999999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}



public class UsersEndpoint
{
  [Fact(DisplayName = "200: GET /users/{id}")]
  public async Task GetUserById_200()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/users/1");
    var content = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

    var expectedContent = new User
    {
      Id = 1,
      Username = "froggie",
    };

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Equivalent(expectedContent, content);
  }
  [Fact(DisplayName = "400: GET /users/{id}")]

  public async Task GetUserById_400()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/users/notanid");

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact(DisplayName = "404: GET /users/{id}")]
  public async Task GetArtById_404()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = await client.GetAsync("/users/999999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }
}

