using MasterData.Domain;
using MasterData.Host.Configuration;
using MasterData.Host.Endpoints;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<DatabaseContext>(_ => _.UseSqlServer(builder.Configuration.GetConnectionString("SQLDatabase")));

builder.Services
    .ConfigureServices()
    .ConfigureSwagger(builder.Configuration)
    .ConfigureAuthentication(builder.Configuration)
    .AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();


// TODO: does not scale, should be done during deployment
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.EnableSwagger(app.Configuration);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("vendors").ConfigureVendorOperations();
//.WithName("GetWeatherForecast")
//.WithOpenApi();

app.Run();
