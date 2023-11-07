namespace CardboardBox.Redis;

/// <summary>
/// The dependency injection extensions for adding redis
/// </summary>
public static class RedisDiExtensions
{
    private static IServiceCollection AddRedisBase(this IServiceCollection services)
    {
        return services
            .AddSingleton<IRedisConnection, RedisConnection>()
            .AddTransient<IRedisService, RedisService>();
    }

    /// <summary>
    /// Adds redis to the service collection. Loads the configuration from an instance of <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRedis(this IServiceCollection services)
    {
        return services.AddRedis<DynamicRedisConfig>();
    }

    /// <summary>
    /// Adds redis to the service collection using the given configuration
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="options">The configuration options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRedis(this IServiceCollection services, ConfigurationOptions options)
    {
        return services.AddRedis(new StaticRedisConfig(options));
    }

    /// <summary>
    /// Adds redis to the service collection using the given connection string
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="connectionString">The connection string to use</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString)
    {
        return services.AddRedis(new StaticRedisConfig(connectionString));
    }

    /// <summary>
    /// Adds redis to the service collection using the given configuration
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="config">The configuration options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRedis(this IServiceCollection services, IRedisConfig config)
    {
        return services
            .AddRedisBase()
            .AddSingleton(config);
    }

    /// <summary>
    /// Adds redis to the service collection using the given configuration type
    /// </summary>
    /// <typeparam name="T">The type of the configuration service</typeparam>
    /// <param name="services">The service collection to add the services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRedis<T>(this IServiceCollection services) where T : class, IRedisConfig
    {
        return services
            .AddRedisBase()
            .AddTransient<IRedisConfig, T>();
    }

    /// <summary>
    /// Converts a string to a redis value
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>The converted value</returns>
    public static RedisValue Convert(this string value)
    {
        return new RedisValue(value);
    }

    /// <summary>
    /// Converts the given strings to redis values
    /// </summary>
    /// <param name="values">The values to convert</param>
    /// <returns>The converted redis values</returns>
    public static RedisValue[] Convert(this IEnumerable<string> values)
    {
        return values.Select(Convert).ToArray();
    }
}
