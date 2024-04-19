using Microsoft.OpenApi.Models;
using ArtsieApi.Models;
using ArtsieApi.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ArtsieDatabaseSettings>(
builder.Configuration.GetSection("ArtsieDatabase"));

builder.Services.AddSingleton<ArtsieService>();

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Art API", Description = "Browse some beautiful art", Version = "v1" });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

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

// app.MapGet("/art/{id}", (int id) =>
// {
//     var art = ArtDB.GetArtById(id);
//     if (art == null)
//     {
//         // Return 404 response if art is not found
//         return Results.NotFound("Art not found.");
//     }
//     else
//     {
//         // Return the art object
//         return Results.Ok(art);
//     }
// }).WithOpenApi(operation =>
//     {
//         operation.Summary = "Returns art data for given Art ID";
//         operation.Description = "Returns an object containing the name, artist, description and image url of the requested art.";
//         operation.Parameters[0].Description = "An integer that references the Art ID of the requested art";
//         return operation;

//     }).Produces<Art>(StatusCodes.Status200OK)
//     .Produces(StatusCodes.Status400BadRequest)
//     .Produces(StatusCodes.Status404NotFound);

// app.MapGet("/art/{id}/comments", (int id) =>
// {
//     var comments = CommentsDB.GetCommentsByArtId(id);
//     if (comments == null)
//     {
//         // Return 404 response if art is not found
//         return Results.NotFound("Art not found.");
//     }
//     else
//     {
//         // Return the comment array
//         return Results.Ok(comments);
//     }
// }).WithOpenApi(operation =>
//     {
//         operation.Summary = "Returns comments for given Art ID";
//         operation.Description = "Returns an array of comment objects associated with the requested Art ID. Returns an empty array if there are no comments associated with the requested Art ID.";
//         operation.Parameters[0].Description = "An integer that references the Art ID associated with the requested comments";
//         return operation;

//     }).Produces<List<Comment>>(StatusCodes.Status200OK)
//     .Produces(StatusCodes.Status400BadRequest)
//     .Produces(StatusCodes.Status404NotFound); ;
// app.MapPost("/art/{id}/comments", (int id, Comment comment) =>
// {
//     var response = CommentsDB.CreateComment(id, comment);
//         if (response == null)
//     {
//         // Return 404 response if art is not found
//         return Results.NotFound("Art not found.");
//     }
//     else
//     {
//         // Return successful 201 response
//         return Results.Created($"/comments/{response.Id}", response);
//     }
// }).WithOpenApi(operation =>
//     {
//         operation.Summary = "Creates comment for given Art ID";
//         operation.Description = "Returns the created comment object with unique Id and Likes property initialised to zero.";
//         operation.Parameters[0].Description = "An integer that references the Art ID associated with the created comment";

//         return operation;

//     }).Accepts<Comment>("application/json")
//     .Produces<Comment>(StatusCodes.Status201Created)
//     .Produces(StatusCodes.Status400BadRequest);
// app.MapPut("/comments/{id}", (int id, Comment update) =>
// {
//     var response = CommentsDB.UpdateCommentLikes(id, update);
//     if (response == null)
//     {
//         // Return 404 response if comment is not found
//         return Results.NotFound("Comment not found.");
//     }
//     else
//     {
//         // Return successful 201 response
//         return Results.Created($"/comments/{id}", response);
//     }
// }).WithOpenApi(operation =>
//     {
//         operation.Summary = "Updates likes for given Comment";
//         operation.Description = "Returns the updated comment object.";
//         operation.Parameters[0].Description = "An integer that references the Comment ID associated with the updated comment";

//         return operation;

//     }).Accepts<Comment>("application/json")
//     .Produces<Comment>(StatusCodes.Status201Created)
//     .Produces(StatusCodes.Status404NotFound);
// app.MapDelete("/comments/{id}", (int id) =>
// {
//     var response = CommentsDB.RemoveComment(id);
//     if (response == null)
//     {
//         // Return 404 response if comment is not found
//         return Results.NotFound("Comment not found.");
//     }
//     else
//     {
//         // Return successful 204 response
//         return Results.NoContent();
//     }
// }).WithOpenApi(operation =>
//     {
//         operation.Summary = "Deletes comment for given Comment ID";
//         operation.Description = "Returns no content.";
//         operation.Parameters[0].Description = "An integer that references the Comment ID of the requested comment to delete";
//         return operation;

//     }).Produces(StatusCodes.Status204NoContent)
//     .Produces(StatusCodes.Status400BadRequest)
//     .Produces(StatusCodes.Status404NotFound);
// app.MapGet("/users/{id}", (int id) =>
// {
//     var user = UsersDB.GetUserById(id);
//     if (user == null)
//     {
//         // Return 404 response if user is not found
//         return Results.NotFound("User not found.");
//     }
//     else
//     {
//         // Return the user object
//         return Results.Ok(user);
//     }
// }).WithOpenApi(operation =>
//     {
//         operation.Summary = "Returns user data for given User ID";
//         operation.Description = "Returns an object containing the username of the requested user.";
//         operation.Parameters[0].Description = "An integer that references the User ID of the requested user";
//         return operation;

//     }).Produces<User>(StatusCodes.Status200OK)
//     .Produces(StatusCodes.Status400BadRequest)
//     .Produces(StatusCodes.Status404NotFound);

// app.MapGet("/{*unknown}", (string unknown) => Results.NotFound("Page not found.")).WithOpenApi(operation =>
//     {
//         operation.Summary = "Returns 404 for unknown endpoints";
//         operation.Description = "Returns 404 Not Found for all unknown endpoints.";
//         operation.Parameters[0].Description = "Any unknown path";
//         return operation;

//     }).Produces(StatusCodes.Status404NotFound);

app.Run();

public partial class Program { }