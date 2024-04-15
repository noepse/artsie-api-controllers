using Microsoft.OpenApi.Models;
using Artsie.DB;

var builder = WebApplication.CreateBuilder(args);
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Art API", Description = "Browse some beautiful art", Version = "v1" });
});
    
var app = builder.Build();
    
app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Art API V1");
});
    
app.MapGet("/", () => "Hello World!");

app.MapGet("/art/{id}", (int id) =>
{
    var art = ArtDB.GetArtById(id);
    if (art == null)
    {
        // Return 404 response if art is not found
        return Results.NotFound("Art not found.");
    }
    else
    {
        // Return the art object
        return Results.Ok(art);
    }
});
app.MapGet("/art/{id}/comments", (int id) => CommentsDB.GetCommentsByArtId(id));
app.MapPost("/comments", (Comment comment) => CommentsDB.CreateComment(comment));
app.MapPut("/comments", (Comment comment) => CommentsDB.UpdateComment(comment));
app.MapDelete("/comments/{id}", (int id) => CommentsDB.RemoveComment(id));
    
app.Run();

public partial class Program { }