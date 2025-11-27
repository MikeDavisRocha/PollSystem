using Microsoft.EntityFrameworkCore;
using PollSystem.API.Data;
using PollSystem.API.Features.Polls;
using PollSystem.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true) // Allow any origin
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors();

app.MapHub<PollHub>("/pollHub");

app.MapPollEndpoints();

app.MapGet("/", () => "PollSystem API is running!");

app.Run();
