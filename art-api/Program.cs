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

app.MapGet("/", () => "Hello World!")
.WithOpenApi(operation =>
    {
        operation.Summary = "Returns the root endpoint";
        operation.Description = "Returns Hello World!";

        return operation;
    });

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
}).WithOpenApi(operation =>
    {
        operation.Summary = "Returns art data for given Art ID";
        operation.Description = "Returns an object containing the name, artist, description and image url of the requested art.";
        operation.Parameters[0].Description = "An integer that references the Art ID of the requested art";
        return operation;

    }).Produces<Art>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/art/{id}/comments", (int id) =>
{
    var comments = CommentsDB.GetCommentsByArtId(id);
    if (comments == null)
    {
        // Return 404 response if art is not found
        return Results.NotFound("Art not found.");
    }
    else
    {
        // Return the comment array
        return Results.Ok(comments);
    }
}).WithOpenApi(operation =>
    {
        operation.Summary = "Returns comments for given Art ID";
        operation.Description = "Returns an array of comment objects associated with the requested Art ID. Returns an empty array if there are no comments associated with the requested Art ID..";
        operation.Parameters[0].Description = "An integer that references the Art ID associated with the requested comments";
        return operation;

    }).Produces<List<Comment>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);;
app.MapPost("/comments", (Comment comment) => CommentsDB.CreateComment(comment));
app.MapPut("/comments", (Comment comment) => CommentsDB.UpdateComment(comment));
app.MapDelete("/comments/{id}", (int id) => CommentsDB.RemoveComment(id));
app.MapGet("/users/{id}", (int id) =>
{
    var user = UsersDB.GetUserById(id);
    if (user == null)
    {
        // Return 404 response if user is not found
        return Results.NotFound("User not found.");
    }
    else
    {
        // Return the user object
        return Results.Ok(user);
    }
}).WithOpenApi(operation =>
    {
        operation.Summary = "Returns user data for given User ID";
        operation.Description = "Returns an object containing the username of the requested user.";
        operation.Parameters[0].Description = "An integer that references the User ID of the requested user";
        return operation;

    }).Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

app.Run();

public partial class Program { }