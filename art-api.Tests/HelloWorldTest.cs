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

    var response = await client.GetStringAsync("/");

    Assert.Equal("Hello World!", response);
  }
}
public class ArtEndpoint
{
  [Fact (DisplayName = "200: GET /art/{id}")]
  public async Task TestArtById()
  {
    await using var application = new WebApplicationFactory<Program>();
    using var client = application.CreateClient();

    var response = JsonConvert.DeserializeObject<Art>(await client.GetStringAsync("/art/1"));
    var rawResponse = await client.GetStringAsync("/art/1");
    var expectedResponse = new Art
    {
      Id = 1,
      Name = "Cleopatra decorating the Tomb of Mark Anthony",
      Artist = "Angelica Kauffman",
      Description = "The scene portrays Cleopatra in a moment of mourning and love, as she decorates the tomb of Mark Antony, her lover and ally, who had committed suicide after being defeated by Octavian (later Emperor Augustus) in the Battle of Actium in 31 BC. Cleopatra, with a look of sorrow and determination, is shown placing a wreath on Antony's tomb, surrounded by mourners and attendants.",
    };
Console.Write(HttpStatusCode.OK);
    Assert.Equivalent(expectedResponse, response);
  }
}