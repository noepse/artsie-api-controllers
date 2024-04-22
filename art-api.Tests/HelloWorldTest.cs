// namespace art_api.Tests;

// using Microsoft.AspNetCore.Mvc.Testing;
// using Artsie.DB;
// using Newtonsoft.Json;
// using System.Net;
// using System.Text;

// public class Root
// {
//   [Fact(DisplayName= "200: GET /")]
//   public async Task TestRootEndpoint()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     var response = await client.GetAsync("/");
//     var content = await response.Content.ReadAsStringAsync();

//     Assert.Equal("Hello World!", content);
//     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//   }
// [Fact(DisplayName = "404: GET /{*unknown}")]
//   public async Task GetUnknownEndpoints_404()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     var response = await client.GetAsync("/unknown");

//     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//   }
// }
// public class ArtEndpoint
// {
//   [Fact(DisplayName = "200: GET /art/{id}")]
//   public async Task GetArtById_200()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     var response = await client.GetAsync("/art/1");
//     var content = JsonConvert.DeserializeObject<Art>(await response.Content.ReadAsStringAsync());

//     var expectedContent = new Art
//     {
//       Id = 1,
//       Name = "Cleopatra decorating the Tomb of Mark Anthony",
//       Artist = "Angelica Kauffman",
//       Description = "The scene portrays Cleopatra in a moment of mourning and love, as she decorates the tomb of Mark Antony, her lover and ally, who had committed suicide after being defeated by Octavian (later Emperor Augustus) in the Battle of Actium in 31 BC. Cleopatra, with a look of sorrow and determination, is shown placing a wreath on Antony's tomb, surrounded by mourners and attendants.",
//     };

//     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//     Assert.Equivalent(expectedContent, content);
//   }
//   [Fact(DisplayName = "400: GET /art/{id}")]

//   public async Task GetArtById_400()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     var response = await client.GetAsync("/art/notanid");

//     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//   }

//   [Fact(DisplayName = "404: GET /art/{id}")]
//   public async Task GetArtById_404()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     var response = await client.GetAsync("/art/999999");

//     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//   }
//   [Fact(DisplayName = "200: GET /art/{id}/comments")]
//   public async Task GetCommentsByArtId_200()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

//     var response = await client.GetAsync("/art/1/comments");
//     List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());

//     var expectedContent = new[]{
//       new Comment{ Id=1, ArtId=1, Author="froggie", Body="Nice art!", Likes = 0 }};

//     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//     Assert.Equivalent(expectedContent, content);
//   }
//   [Fact(DisplayName = "400: GET /art/{id}/comments")]
//   public async Task GetCommentsByArtId_400()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

//     var response = await client.GetAsync("/art/notanid/comments");

//     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//   }
//   [Fact(DisplayName = "404: GET /art/{id}/comments")]
//   public async Task GetCommentsByArtId_404()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

//     var response = await client.GetAsync("/art/999999/comments");

//     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//   }
//    [Fact(DisplayName = "201: POST /art/{id}/comments")]
//   public async Task PostComment_201()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

//     var update = new Comment
//     {
//       Author = "froggie",
//       Body = "Neato!",
//     };

//     var expectedOutput = new Comment
//     {
//       ArtId = 2,
//       Author = "froggie",
//       Body = "Neato!",
//       Likes = 0,
//     };

//     var json = JsonConvert.SerializeObject(update);
//     var data = new StringContent(json, Encoding.UTF32, "application/json");

//     var response = await client.PostAsync("/art/2/comments", data);
//     var comments = await client.GetAsync("/art/2/comments");

//     List<Comment>? content = JsonConvert.DeserializeObject<List<Comment>>(await comments.Content.ReadAsStringAsync());

//     Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//     Assert.Equal(content[0].ArtId, expectedOutput.ArtId);
//     Assert.Equal(content[0].Author, expectedOutput.Author);
//     Assert.Equal(content[0].Body, expectedOutput.Body);
//     Assert.Equal(content[0].Likes, expectedOutput.Likes);
//     Assert.Single(content);
//     Assert.IsType<int>(content[0].Id);
//   }
  
//   [Fact(DisplayName = "404: PUT /art/{id}/comments")]
//   public async Task PostComment_404()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

//         var update = new Comment
//     {
//       Author = "froggie",
//       Body = "Neato!",
//     };

//     var json = JsonConvert.SerializeObject(update);
//     var data = new StringContent(json, Encoding.UTF32, "application/json");

//     var response = await client.PostAsync("/art/99999/comments", data);

//     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//   }
//   [Fact(DisplayName = "201: PUT /comments/{id}")]
//   public async Task UpdateCommentLikes_201()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

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

//     // Reseed comments
//     CommentsDB.Seed();

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
// }

// public class CommentsEndpoint
// {
//   [Fact(DisplayName = "204: DELETE /comments/{id}")]
//   public async Task DeleteCommentById_204()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

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

//     // Reseed comments
//     CommentsDB.Seed();

//     var response = await client.DeleteAsync("/comments/notanid");

//     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//   }

//   [Fact(DisplayName = "404: DELETE /comments/{id}")]
//   public async Task DeleteCommentById_404()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     // Reseed comments
//     CommentsDB.Seed();

//     var response = await client.DeleteAsync("/comments/999999");

//     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//   }
  
// }

// public class UsersEndpoint
// {
//   [Fact(DisplayName = "200: GET /users/{id}")]
//   public async Task GetUserById_200()
//   {
//     await using var application = new WebApplicationFactory<Program>();
//     using var client = application.CreateClient();

//     var response = await client.GetAsync("/users/1");
//     var content = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

//     var expectedContent = new User
//     {
//       Id = 1,
//       Username = "froggie",
//     };

//     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//     Assert.Equivalent(expectedContent, content);
//   }
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
// }

