using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

builder.AddProject<Projects.MasterData_Host>("masterdata-host")
    .WithReference(redis);

builder.Build().Run();
