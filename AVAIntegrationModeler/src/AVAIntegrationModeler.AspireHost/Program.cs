var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AVAIntegrationModeler_API>("API");

builder.Build().Run();
