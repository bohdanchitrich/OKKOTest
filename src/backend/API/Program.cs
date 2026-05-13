using API.ActionFilters;
using API.DTOs;
using API.Middlewares;
using Application.Services;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ApiSignatureFilter>();


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(new ErrorResponse(
            "validation_error",
            "Invalid request body."));
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    });
}

#if DEBUG

    var staticKey = builder.Configuration["ApiSignature:StaticKey"];
    var requestDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    using var sha256 = System.Security.Cryptography.SHA256.Create();

    var raw = $"{staticKey}{requestDate}";
    var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
    var hash = sha256.ComputeHash(bytes);
    var apiSignature = Convert.ToHexString(hash);

    Console.WriteLine("=== Test login request ===");
    Console.WriteLine($$"""
        {
          "apiSignature": "{{apiSignature}}",
          "requestDate": {{requestDate}},
          "login": "admin",
          "password": "admin"
        }
        """);
#endif


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.MapControllers();


app.Run();


//For tests
public partial class Program { }
