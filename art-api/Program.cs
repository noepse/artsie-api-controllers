using Microsoft.OpenApi.Models;
using ArtsieApi.Models;
using ArtsieApi.Services;


var builder = WebApplication.CreateBuilder(args);

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


app.MapGet("/{*unknown}", (string unknown) => Results.NotFound("Page not found.")).WithOpenApi(operation =>
    {
        operation.Summary = "Returns page not found for unknown endpoints";
        operation.Description = "Returns 404 Page not found for all unknown endpoints.";
        operation.Parameters[0].Description = "Any unknown path";
        return operation;

    }).Produces(StatusCodes.Status404NotFound);

app.Run();

public partial class Program { }