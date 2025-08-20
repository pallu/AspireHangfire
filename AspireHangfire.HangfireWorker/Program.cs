using AspireHangfire.JobLibrary;
using Hangfire;
using Hangfire.Redis.StackExchange;
namespace AspireHangfire.HangfireWorker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();
        builder.Services.AddTransient<IPrintingJobs,PrintingJobs>();
        builder.Services.AddHangfire((provider,config)=>
        {
            var conn = builder.Configuration.GetConnectionString("cache");
            //var mux = provider.GetRequiredService<IConnectionMultiplexer>();
            // Use the Redis storage for Hangfire.
            config.UseRedisStorage(conn);
            // Configure the server to use a custom worker count.
            //options.WorkerCount = 1;
        });
        builder.Services.AddHangfireServer();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}