namespace CardboardBox.Redis;

/// <summary>
/// A singleton representing the connection to redis
/// </summary>
public interface IRedisConnection
{
    /// <summary>
    /// Creates the connection to redis
    /// </summary>
    /// <returns>The connection to redis</returns>
    Task<ConnectionMultiplexer> Connect();
}

/// <summary>
/// The concrete implementation of <see cref="IRedisConnection"/>
/// </summary>
public class RedisConnection : IRedisConnection
{
    private readonly IRedisConfig _config;
    private ConnectionMultiplexer? _connection;

    /// <summary>
    /// The Dependency Injection constructor
    /// </summary>
    /// <param name="config">The configuration options for the redis connection</param>
    public RedisConnection(IRedisConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Creates the connection to redis using the configuration options from <see cref="IRedisConfig"/>
    /// Lazy loads the <see cref="ConnectionMultiplexer"/>, creating it if it doesn't already exists.
    /// </summary>
    /// <returns>The connection to redis</returns>
    public async Task<ConnectionMultiplexer> Connect()
    {
        return _connection ??= await ConnectionMultiplexer.ConnectAsync(_config.Options);
    }
}