namespace CardboardBox.Redis;

using Json;

/// <summary>
/// A transient service the allows for easy interfacing with redis
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// Prefixes the given key with the appropriate prefix
    /// </summary>
    /// <param name="key">The key to prefix</param>
    /// <param name="data">Whether to apply the <see cref="IRedisConfig.DataPrefix"/> or the <see cref="IRedisConfig.EventsPrefix"/></param>
    /// <returns>The prefixed key</returns>
    string Prefix(string key, bool data = true);

    #region Redis Database
    /// <summary>
    /// Gets a connection to the redis database instance
    /// </summary>
    /// <returns>Gets the redis database instance</returns>
    Task<IDatabase> GetDatabase();

    /// <summary>
    /// Gets a string from the redis instance
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <returns>The value of the variable or null if it's not found</returns>
    Task<string?> Get(string key);

    /// <summary>
    /// Gets the JSON value from the redis instance
    /// </summary>
    /// <typeparam name="T">The type of data</typeparam>
    /// <param name="key">The key of the variable</param>
    /// <param name="def">The default value to return if no instance is found</param>
    /// <returns>The value of the variable or the default option if it's not found</returns>
    Task<T?> Get<T>(string key, T? def = default);

    /// <summary>
    /// Sets the value of a variable in redis
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <param name="data">The data to set the variable to</param>
    /// <param name="expiry">The expiry to set</param>
    /// <returns>true if the value was set, false if otherwise</returns>
    Task<bool> Set(string key, RedisValue data, TimeSpan? expiry = null);

    /// <summary>
    /// Sets the value of a variable in redis
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <param name="data">The data to set the variable to</param>
    /// <param name="expiry">The expiry to set</param>
    /// <returns>true if the value was set, false if otherwise</returns>
    Task<bool> Set(string key, string data, TimeSpan? expiry = null);

    /// <summary>
    /// Sets the JSON value of a variable in redis
    /// </summary>
    /// <typeparam name="T">The type of data</typeparam>
    /// <param name="key">The key of the variable</param>
    /// <param name="data">The data to set the variable to</param>
    /// <param name="expiry">The expiry to set</param>
    /// <returns>true if the value was set, false if otherwise</returns>
    Task<bool> Set<T>(string key, T data, TimeSpan? expiry = null);

    /// <summary>
    /// Deletes a variable in redis
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <returns>true if the variable was removed</returns>
    Task<bool> Delete(string key);

    /// <summary>
    /// Provides access to redis related list operations
    /// </summary>
    /// <param name="key">The key of the redis list</param>
    /// <returns>The redis list service</returns>
    IRedisList List(string key);

    /// <summary>
    /// Provides access to redis related list operations
    /// </summary>
    /// <typeparam name="T">The type of items in the list</typeparam>
    /// <param name="key">The key of the redis list</param>
    /// <returns>The redis list service</returns>
    IRedisList<T> List<T>(string key);

    /// <summary>
    /// Provides access to redis related hash set operations.
    /// </summary>
    /// <param name="key">The key of the redis hash set</param>
    /// <returns>The redis hash set service</returns>
    IRedisHashSet HashSet(string key);

    /// <summary>
    /// Provides access to redis related hash set operations.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the hash-set (should be string-convertible)</typeparam>
    /// <typeparam name="TValue">The type of values in the hash-set</typeparam>
    /// <param name="key">The key of the redis hash set</param>
    /// <returns>The redis hash set service</returns>
    IRedisHashSet<TKey, TValue> HashSet<TKey, TValue>(string key)
        where TKey : notnull, IConvertible, IFormattable, IComparable<TKey>, IEquatable<TKey>;
    #endregion

    #region Redis Pub/Sub
    /// <summary>
    /// Gets a connection to the redis subscriber instance
    /// </summary>
    /// <returns>The redis subscriber instance</returns>
    Task<ISubscriber> GetSubscriber();

    /// <summary>
    /// Subscribes to the given channel and executes the given action whenever it triggers
    /// </summary>
    /// <param name="channel">The channel to subscribe to</param>
    /// <param name="action">The action to execute whenever a message is received</param>
    /// <returns>A task representing the completion of the subscription</returns>
    Task Subscribe(string channel, Action<RedisChannel, RedisValue> action);

    /// <summary>
    /// Subscribes to the given channel and executes the given action whenever it triggers
    /// </summary>
    /// <typeparam name="T">The type of data the subscription is dealing with</typeparam>
    /// <param name="channel">The channel to subscribe to</param>
    /// <param name="action">The action to execute whenever a message is received</param>
    /// <returns>A task representing the completion of the subscription</returns>
    Task Subscribe<T>(string channel, Action<RedisChannel, T?> action);

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// </summary>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    Task<IObservable<RedisSub>> Subscribe(string channel);

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// </summary>
    /// <typeparam name="T">The type of data the subscription is dealing with</typeparam>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    Task<IObservable<RedisSub<T>>> Subscribe<T>(string channel);

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// The only difference between this method and <see cref="Subscribe(string)"/> is that this one throws away the channel name
    /// </summary>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    Task<IObservable<RedisValue>> Observe(string channel);

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// The only difference between this method and <see cref="Subscribe{T}(string)"/> is that this one throws away the channel name
    /// </summary>
    /// <typeparam name="T">The type of data the subscription is dealing with</typeparam>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    Task<IObservable<T?>> Observe<T>(string channel);

    /// <summary>
    /// Publishes a message to the given channel
    /// </summary>
    /// <param name="channel">The channel to publish to</param>
    /// <param name="value">The value to publish</param>
    /// <returns>A task representing the completion of the publication</returns>
    Task Publish(string channel, RedisValue value);

    /// <summary>
    /// Publishes a message to the given channel
    /// </summary>
    /// <param name="channel">The channel to publish to</param>
    /// <param name="value">The value to publish</param>
    /// <returns>A task representing the completion of the publication</returns>
    Task Publish(string channel, string value);

    /// <summary>
    /// Publishes a message to the given channel
    /// </summary>
    /// <typeparam name="T">The type of data to publish</typeparam>
    /// <param name="channel">The channel to publish to</param>
    /// <param name="value">The value to publish</param>
    /// <returns>A task representing the completion of the publication</returns>
    Task Publish<T>(string channel, T value);

    /// <summary>
    /// Unsubscribes from the given channel
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from</param>
    /// <returns>A task representing the completion of an unsubscribe action</returns>
    Task Unsubscribe(string channel);
    #endregion
}

/// <summary>
/// The concrete implementation of the <see cref="IRedisService"/>
/// </summary>
public class RedisService(
    IRedisConnection _connection,
    IRedisJsonService _json,
    IRedisConfig _config) : IRedisService
{
    private IDatabase? _database;
    private ISubscriber? _subscriber;

    /// <summary>
    /// Prefixes the given key with the appropriate prefix
    /// </summary>
    /// <param name="key">The key to prefix</param>
    /// <param name="data">Whether to apply the <see cref="IRedisConfig.DataPrefix"/> or the <see cref="IRedisConfig.EventsPrefix"/></param>
    /// <returns>The prefixed key</returns>
    public string Prefix(string key, bool data = true)
    {
        return (data ? _config.DataPrefix : _config.EventsPrefix) + key;
    }

    #region Redis Database
    /// <summary>
    /// Gets a connection to the redis database instance
    /// </summary>
    /// <returns>Gets the redis database instance</returns>
    public async Task<IDatabase> GetDatabase()
    {
        return _database ??= (await _connection.Connect()).GetDatabase();
    }

    /// <summary>
    /// Gets a string from the redis instance
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <returns>The value of the variable or null if it's not found</returns>
    public async Task<string?> Get(string key)
    {
        var database = await GetDatabase();
        var value = await database.StringGetAsync(Prefix(key));
        if (value.IsNullOrEmpty) return null;

        return value.ToString();
    }

    /// <summary>
    /// Gets the JSON value from the redis instance
    /// </summary>
    /// <typeparam name="T">The type of data</typeparam>
    /// <param name="key">The key of the variable</param>
    /// <param name="def">The default value to return if no instance is found</param>
    /// <returns>The value of the variable or the default option if it's not found</returns>
    public async Task<T?> Get<T>(string key, T? def = default)
    {
        var value = await Get(key);
        if (string.IsNullOrEmpty(value)) return def;

        return _json.Deserialize<T>(value);
    }

    /// <summary>
    /// Sets the value of a variable in redis
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <param name="data">The data to set the variable to</param>
    /// <param name="expiry">The expiry to set</param>
    /// <returns>true if the value was set, false if otherwise</returns>
    public async Task<bool> Set(string key, RedisValue data, TimeSpan? expiry = null)
    {
        var database = await GetDatabase();
        return await database.StringSetAsync(Prefix(key), data, expiry);
    }

    /// <summary>
    /// Sets the value of a variable in redis
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <param name="data">The data to set the variable to</param>
    /// <param name="expiry">The expiry to set</param>
    /// <returns>true if the value was set, false if otherwise</returns>
    public Task<bool> Set(string key, string data, TimeSpan? expiry = null) => Set(key, data.Convert(), expiry);

    /// <summary>
    /// Sets the JSON value of a variable in redis
    /// </summary>
    /// <typeparam name="T">The type of data</typeparam>
    /// <param name="key">The key of the variable</param>
    /// <param name="data">The data to set the variable to</param>
    /// <param name="expiry">The expiry to set</param>
    /// <returns>true if the value was set, false if otherwise</returns>
    public Task<bool> Set<T>(string key, T data, TimeSpan? expiry = null)
    {
        return Set(key, _json.Serialize(data).Convert(), expiry);
    }

    /// <summary>
    /// Deletes a variable in redis
    /// </summary>
    /// <param name="key">The key of the variable</param>
    /// <returns>true if the variable was removed</returns>
    public async Task<bool> Delete(string key)
    {
        var database = await GetDatabase();
        return await database.KeyDeleteAsync(Prefix(key));
    }

    /// <summary>
    /// Provides access to redis related list operations.
    /// Note: This is a transient dummy object, and instances shouldn't be retained / persisted. Just create a new one.
    /// </summary>
    /// <param name="key">The key of the redis list</param>
    /// <returns>The redis list service</returns>
    public IRedisList List(string key) => new RedisList(this, Prefix(key));

    /// <summary>
    /// Provides access to redis related list operations.
    /// Note: This is a transient dummy object, and instances shouldn't be retained / persisted. Just create a new one.
    /// </summary>
    /// <typeparam name="T">The type of items in the list</typeparam>
    /// <param name="key">The key of the redis list</param>
    /// <returns>The redis list service</returns>
    public IRedisList<T> List<T>(string key) => new RedisList<T>(this, Prefix(key), _json);

    /// <summary>
    /// Provides access to redis related hash set operations.
    /// </summary>
    /// <param name="key">The key of the redis hash set</param>
    /// <returns>The redis hash set service</returns>
    public IRedisHashSet HashSet(string key) => new RedisHashSet(this, Prefix(key));

    /// <summary>
    /// Provides access to redis related hash set operations.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the hash-set (should be string-convertible)</typeparam>
    /// <typeparam name="TValue">The type of values in the hash-set</typeparam>
    /// <param name="key">The key of the redis hash set</param>
    /// <returns>The redis hash set service</returns>
    public IRedisHashSet<TKey, TValue> HashSet<TKey, TValue>(string key)
        where TKey : notnull, IConvertible, IFormattable, IComparable<TKey>, IEquatable<TKey>
    {
        return new RedisHashSet<TKey, TValue>(this, Prefix(key), _json);
    }
    #endregion

    #region Redis Pub/Sub
    /// <summary>
    /// Gets a connection to the redis subscriber instance
    /// </summary>
    /// <returns>The redis subscriber instance</returns>
    public async Task<ISubscriber> GetSubscriber()
    {
        return _subscriber ??= (await _connection.Connect()).GetSubscriber();
    }

    /// <summary>
    /// Subscribes to the given channel and executes the given action whenever it triggers
    /// </summary>
    /// <param name="channel">The channel to subscribe to</param>
    /// <param name="action">The action to execute whenever a message is received</param>
    /// <returns>A task representing the completion of the subscription</returns>
    public async Task Subscribe(string channel, Action<RedisChannel, RedisValue> action)
    {
        var subscriber = await GetSubscriber();
        var key = new RedisChannel(Prefix(channel, false), RedisChannel.PatternMode.Literal);
        await subscriber.SubscribeAsync(key, action);
    }

    /// <summary>
    /// Subscribes to the given channel and executes the given action whenever it triggers
    /// </summary>
    /// <typeparam name="T">The type of data the subscription is dealing with</typeparam>
    /// <param name="channel">The channel to subscribe to</param>
    /// <param name="action">The action to execute whenever a message is received</param>
    /// <returns>A task representing the completion of the subscription</returns>
    public async Task Subscribe<T>(string channel, Action<RedisChannel, T?> action)
    {
        var subscriber = await GetSubscriber();

        await Subscribe(channel, (c, v) =>
        {
            if (v.IsNullOrEmpty)
            {
                action(c, default);
                return;
            }

            var value = _json.Deserialize<T>(v.ToString());
            action(c, value);
        });
    }

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// </summary>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    public async Task<IObservable<RedisSub>> Subscribe(string channel)
    {
        var subject = new Subject<RedisSub>();
        await Subscribe(channel, (cha, val) => subject.OnNext(new RedisSub(cha, val)));
        return subject.AsObservable();
    }

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// </summary>
    /// <typeparam name="T">The type of data the subscription is dealing with</typeparam>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    public async Task<IObservable<RedisSub<T>>> Subscribe<T>(string channel)
    {
        var subject = new Subject<RedisSub<T>>();
        await Subscribe<T>(channel, (cha, val) => subject.OnNext(new RedisSub<T>(cha, val)));
        return subject.AsObservable();
    }

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// The only difference between this method and <see cref="Subscribe(string)"/> is that this one throws away the channel name
    /// </summary>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    public async Task<IObservable<RedisValue>> Observe(string channel)
    {
        return (await Subscribe(channel)).Select(t => t.Value);
    }

    /// <summary>
    /// Subscribes to the given channel and watches for changes
    /// The only difference between this method and <see cref="Subscribe{T}(string)"/> is that this one throws away the channel name
    /// </summary>
    /// <typeparam name="T">The type of data the subscription is dealing with</typeparam>
    /// <param name="channel">The channel to subscribe to</param>
    /// <returns>An observable that triggers whenever the subscription triggers</returns>
    public async Task<IObservable<T?>> Observe<T>(string channel)
    {
        return (await Subscribe<T>(channel)).Select(t => t.Value);
    }

    /// <summary>
    /// Publishes a message to the given channel
    /// </summary>
    /// <param name="channel">The channel to publish to</param>
    /// <param name="value">The value to publish</param>
    /// <returns>A task representing the completion of the publication</returns>
    public async Task Publish(string channel, RedisValue value)
    {
        var subscriber = await GetSubscriber();
        var key = new RedisChannel(Prefix(channel, false), RedisChannel.PatternMode.Literal);
        await subscriber.PublishAsync(key, value);
    }

    /// <summary>
    /// Publishes a message to the given channel
    /// </summary>
    /// <param name="channel">The channel to publish to</param>
    /// <param name="value">The value to publish</param>
    /// <returns>A task representing the completion of the publication</returns>
    public Task Publish(string channel, string value) => Publish(channel, value.Convert());

    /// <summary>
    /// Publishes a message to the given channel
    /// </summary>
    /// <typeparam name="T">The type of data to publish</typeparam>
    /// <param name="channel">The channel to publish to</param>
    /// <param name="value">The value to publish</param>
    /// <returns>A task representing the completion of the publication</returns>
    public Task Publish<T>(string channel, T value)
    {
        return Publish(channel, _json.Serialize(value).Convert());
    }

    /// <summary>
    /// Unsubscribes from the given channel
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from</param>
    /// <returns>A task representing the completion of the unsubscribe action</returns>
    public async Task Unsubscribe(string channel)
    {
        var subscriber = await GetSubscriber();
        var key = new RedisChannel(Prefix(channel, false), RedisChannel.PatternMode.Literal);
        await subscriber.UnsubscribeAsync(key);
    }
    #endregion
}
