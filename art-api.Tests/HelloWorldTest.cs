namespace art_api.Tests;

using Microsoft.AspNetCore.Mvc.Testing;

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