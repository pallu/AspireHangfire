using StackExchange.Redis;

namespace AspireHangfire.HangfireWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnectionMultiplexer _redis;
    private ISubscriber _subscriber;

    public Worker(ILogger<Worker> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        await server.ConfigSetAsync("notify-keyspace-events", "Ex");

        // Subscribe to expiration events
        _subscriber = _redis.GetSubscriber();
        await _subscriber.SubscribeAsync(
            new RedisChannel("__keyevent@0__:expired", RedisChannel.PatternMode.Pattern),
            (channel, key) =>
            {
                Console.WriteLine($"Key expired: {key}");
                // Handle expiration
            });

        while (!stoppingToken.IsCancellationRequested)
        {
            //if (_logger.IsEnabled(LogLevel.Information))
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //}
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _subscriber.UnsubscribeAllAsync();
        await base.StopAsync(cancellationToken);
    }
}
