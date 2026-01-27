var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("aspire-hangfire-env");
var cache = builder.AddRedis("cache").WithRedisInsight().WithPersistence();


var apiService = builder.AddProject<Projects.AspireHangfire_ApiService>("apiservice");
// The shared job library project

var hangfireWorker = builder.AddProject<Projects.AspireHangfire_HangfireWorker>("aspirehangfire-hangfireworker")
    .WithReference(cache)
    .WaitFor(cache);

builder.AddProject<Projects.AspireHangfire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService)
    .WaitFor(hangfireWorker);



builder.Build().Run();
