using MasterData.Domain;
using MasterData.Host.Configuration;
using MasterData.Host.Endpoints;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(_ => _.UseSqlServer(builder.Configuration.GetConnectionString("SQLDatabase")));

builder.Services.ConfigureServices();

var app = builder.Build();

// TODO: does not scale, should be done during deployment
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("vendors").ConfigureVendorOperations();
//.WithName("GetWeatherForecast")
//.WithOpenApi();

app.Run();
