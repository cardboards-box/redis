namespace CardboardBox.Redis;

/// <summary>
/// The configuration values from the redis connection
/// </summary>
public interface IRedisConfig
{
    /// <summary>
    /// The configuration options for the redis connection
    /// </summary>
    ConfigurationOptions Options { get; }

    /// <summary>
    /// The optional prefix to prepend to all data elements
    /// </summary>
    string DataPrefix { get; }

    /// <summary>
    /// The optional prefix to prepend to all pub/sub events
    /// </summary>
    string EventsPrefix { get; }
}

internal class StaticRedisConfig : IRedisConfig
{
    public ConfigurationOptions Options { get; }

    public string DataPrefix { get; }

    public string EventsPrefix { get; }

    public StaticRedisConfig(ConfigurationOptions options, string eventsPrefix = "", string dataPrefix = "")
    {
        Options = options;
        EventsPrefix = eventsPrefix;
        DataPrefix = dataPrefix;
    }

    public StaticRedisConfig(string connection, string eventsPrefix = "", string dataPrefix = "")
    {
        Options = ConfigurationOptions.Parse(connection);
        EventsPrefix = eventsPrefix;
        DataPrefix = dataPrefix;
    }
}

internal class DynamicRedisConfig : IRedisConfig
{
    private readonly IConfiguration _config;

    public string ConnectionString => _config["Redis:Connection"] ?? throw new ArgumentNullException("Redis:Connection", "Redis connection string cannot be null");

    public string DataPrefix => _config["Redis:Prefix:Data"] ?? string.Empty;

    public string EventsPrefix => _config["Redis:Prefix:Events"] ?? string.Empty;

    public ConfigurationOptions Options => ConfigurationOptions.Parse(ConnectionString);

    public DynamicRedisConfig(IConfiguration config)
    {
        _config = config;
    }
}
