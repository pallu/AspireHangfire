using AspireHangfire.Web;
using AspireHangfire.Web.Components;
using Hangfire;
using Hangfire.Redis.StackExchange;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);


// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

builder.AddRedisClient(connectionName: "cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

builder.Services.AddHangfire((provider,config)
    =>
    {
        var conn = builder.Configuration.GetConnectionString("cache");
        var mux = provider.GetRequiredService<IConnectionMultiplexer>();
        // Use the Redis storage for Hangfire.
        //config.UseRedisStorage("cache");
        config.UseRedisStorage(mux);
        // Configure the server to use a custom worker count.
        //options.WorkerCount = 1;
    });
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();
app.MapHangfireDashboard();
//app.MapGet("/enqueue-job", async (IBackgroundJobClient backgroundJobClient) =>
//{
//    //BackgroundJob.Enqueue(() => Console.WriteLine("This job runs immediately from BackgroundJob!"));
//    // Enqueue a job to run immediately.
//    backgroundJobClient.Enqueue(() => Console.WriteLine("This job runs immediately from BackgroundJobClient!"));
//    return Results.Ok("Job enqueued!");
//});
app.Run();
