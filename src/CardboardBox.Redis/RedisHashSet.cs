namespace CardboardBox.Redis;

/// <summary>
/// A collection of shared redis hash set operations
/// </summary>
public interface IRedisHashSetShared
{
    /// <summary>
    /// The base key of the hash-set in redis
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the number of entries in the hash-set.
    /// </summary>
    /// <returns>The number of entries in the collection</returns>
    Task<long> Count();

    /// <summary>
    /// Clears the entire hash-set, removing all entries.
    /// </summary>
    Task Clear();
}

/// <summary>
/// Provides access to redis hash set operations
/// </summary>
public interface IRedisHashSet : IRedisHashSetShared
{
    /// <summary>
    /// Sets the value of the given key in the hash-set.
    /// </summary>
    /// <param name="key">The key of the item</param>
    /// <param name="value">The value of the item</param>
    Task Set(RedisValue key, RedisValue value);

    /// <summary>
    /// Gets the value of the given key in the hash-set.
    /// </summary>
    /// <param name="key">The key of the item</param>
    /// <returns>The value of the item</returns>
    Task<RedisValue> Get(RedisValue key);

    /// <summary>
    /// Gets the values of the given keys in the hash-set.
    /// </summary>
    /// <param name="keys">The keys of the hash set</param>
    /// <returns>All of the values</returns>
    Task<RedisValue[]> Get(params RedisValue[] keys);

    /// <summary>
    /// Deletes the given key from the hash-set.
    /// </summary>
    /// <param name="key">The key to delete</param>
    /// <returns>Whether or not the key was deleted</returns>
    Task<bool> Delete(RedisValue key);

    /// <summary>
    /// Whether or not the given key exists in the hash-set
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>Whether or not the key exists</returns>
    Task<bool> Exists(RedisValue key);

    /// <summary>
    /// Gets the value of the given key and deletes the entry in the hash-set
    /// </summary>
    /// <param name="key">The key of the item</param>
    /// <returns>The value of the item</returns>
    Task<RedisValue> GetDelete(RedisValue key);

    /// <summary>
    /// Gets all of the items in the hash-set.
    /// </summary>
    /// <returns>All of the items in the hash set</returns>
    Task<IEnumerable<KeyValuePair<RedisValue, RedisValue>>> All();
}

internal class RedisHashSet(
    IRedisService _redis,
    string _key) : IRedisHashSet
{
    public string Key => _redis.Prefix(_key);

    public async Task<IEnumerable<KeyValuePair<RedisValue, RedisValue>>> All()
    {
        var db = await _redis.GetDatabase();
        return (await db.HashGetAllAsync(Key))
            .Select(t => new KeyValuePair<RedisValue, RedisValue>(t.Name, t.Value));
    }

    public async Task<RedisValue> Get(RedisValue key)
    {
        var db = await _redis.GetDatabase();
        return await db.HashGetAsync(Key, key);
    }

    public async Task<RedisValue[]> Get(params RedisValue[] keys)
    {
        var db = await _redis.GetDatabase();
        return await db.HashGetAsync(Key, keys);
    }

    public async Task<RedisValue> GetDelete(RedisValue key)
    {
        var db = await _redis.GetDatabase();
        var value = await db.HashGetAsync(Key, key);
        if (!value.IsNull)
            await db.HashDeleteAsync(Key, key);
        return value;
    }

    public async Task Set(RedisValue key, RedisValue value)
    {
        var db = await _redis.GetDatabase();
        await db.HashSetAsync(Key, key, value);
    }

    public async Task<bool> Exists(RedisValue key)
    {
        var db = await _redis.GetDatabase();
        return await db.HashExistsAsync(Key, key);
    }

    public async Task<bool> Delete(RedisValue key)
    {
        var db = await _redis.GetDatabase();
        return await db.HashDeleteAsync(Key, key);
    }

    public async Task<long> Count()
    {
        var db = await _redis.GetDatabase();
        return await db.HashLengthAsync(Key);
    }

    public async Task Clear()
    {
        var db = await _redis.GetDatabase();
        await db.KeyDeleteAsync(Key);
    }
}
