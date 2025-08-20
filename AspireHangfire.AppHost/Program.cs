var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisInsight().WithPersistence();


var apiService = builder.AddProject<Projects.AspireHangfire_ApiService>("apiservice");

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
