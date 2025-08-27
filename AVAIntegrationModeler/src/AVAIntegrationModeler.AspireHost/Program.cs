var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AVAIntegrationModeler_Web>("web");

builder.Build().Run();
