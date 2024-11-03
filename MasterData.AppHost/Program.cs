var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MasterData_Host>("masterdata-host");

builder.Build().Run();
