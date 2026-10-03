using Microsoft.EntityFrameworkCore;

using RoomBookingApp.Core.Processors;
using RoomBookingApp.Core.Services;
using RoomBookingApp.Persistence;
using RoomBookingApp.Persistence.Respositories;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<RoomBookingDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("RoomBooking")
                  ?? "Data Source=roombooking.db");
});

builder.Services.AddScoped<IRoomBookingService, RoomBookingService>();
builder.Services.AddScoped<IRoomBookingRequestProcessor, RoomBookingRequestProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference("documentation", options =>
    {
        options.Title = "RoomBookingService";
        options.Layout = ScalarLayout.Modern;
        options.Theme = ScalarTheme.Solarized;
        options.Metadata = new Dictionary<string, string>()
    {
        { "version", "1.0" },
        { "description", "This is a sample API documentation" }
    };
        options.EnabledClients = new ScalarClient[]
        {
        ScalarClient.Libcurl,
        ScalarClient.RestSharp,
        };
        options.DarkMode = true;
        options.CustomCss = ".scalar-api-reference .scalar-api-reference-header { background-color: #333; }";
        options.EnabledTargets = new ScalarTarget[]
        {
        ScalarTarget.Http,
        ScalarTarget.CSharp
        };
        options.Authentication = new ScalarAuthenticationOptions()
        {
            ApiKey = new ApiKeyOptions()
            {
                Token = "myToken"
            }
        };
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
