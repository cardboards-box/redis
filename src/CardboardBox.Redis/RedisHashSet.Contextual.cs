namespace CardboardBox.Redis;

/// <summary>
/// Providers access to redis hash set operations of a given type
/// </summary>
/// <typeparam name="TKey">The type of the key of the hash-set (should be string-convertible)</typeparam>
/// <typeparam name="TValue">The type of values in the hash-set</typeparam>
public interface IRedisHashSet<TKey, TValue> : IRedisHashSetShared
    where TKey: notnull, IConvertible, IFormattable, IComparable<TKey>, IEquatable<TKey>
{
    /// <summary>
    /// Sets the value of the given key in the hash-set.
    /// </summary>
    /// <param name="key">The key of the item in the hash-set</param>
    /// <param name="value">The value of the key in the hash-set</param>
    Task Set(TKey key, TValue value);

    /// <summary>
    /// Gets the value of the given key in the hash-set.
    /// </summary>
    /// <param name="key">The key of the item in the hash-set</param>
    /// <returns>The value of the item or null if it doesn't exist</returns>
    Task<TValue?> Get(TKey key);

    /// <summary>
    /// Gets the values of the given keys in the hash-set
    /// </summary>
    /// <param name="keys">The keys of the hash set</param>
    /// <returns>The values of the hash-set</returns>
    Task<TValue?[]> Get(params TKey[] keys);

    /// <summary>
    /// Deletes the given key from the hash-set.
    /// </summary>
    /// <param name="key">The key to delete</param>
    /// <returns>Whether or not the key was deleted</returns>
    Task<bool> Delete(TKey key);

    /// <summary>
    /// Gets the value of the given key and deletes the entry in the hash-set
    /// </summary>
    /// <param name="key">The key of the item</param>
    /// <returns>The value of the item or null if the key doesn't exist</returns>
    Task<TValue?> GetDelete(TKey key);

    /// <summary>
    /// Gets all of the items in the hash-set.
    /// </summary>
    /// <returns>All of the items in the hash set</returns>
    Task<IEnumerable<KeyValuePair<TKey, TValue?>>> All();

    /// <summary>
    /// Whether or not the given key exists in the hash-set
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>Whether or not the key exists</returns>
    Task<bool> Exists(TKey key);
}

internal class RedisHashSet<TKey, TValue>(
    IRedisService _redis,
    string _prefix,
    IRedisJsonService _json) : RedisHashSet(_redis, _prefix), IRedisHashSet<TKey, TValue>
    where TKey : notnull, IConvertible, IFormattable, IComparable<TKey>, IEquatable<TKey>
{
    public TKey ConvertKey(RedisValue value)
    {
        if (value.IsNullOrEmpty)
            throw new ArgumentNullException(nameof(value), "Cannot convert a null or empty value to a key.");
        return (TKey)Convert.ChangeType(value.ToString(), typeof(TKey));
    }

    public RedisValue ConvertKey(TKey key)
    {
        return key.ToString();
    }

    public TValue? ConvertValue(RedisValue value)
    {
        if (value.IsNullOrEmpty)
            return default;
        return _json.Deserialize<TValue>(value.ToString());
    }

    public RedisValue ConvertValue(TValue value)
    {
        return _json.Serialize(value);
    }

    public new Task<IEnumerable<KeyValuePair<TKey, TValue?>>> All()
    {
        return base.All()
            .ContinueWith(task => task.Result
                .Select(entry => new KeyValuePair<TKey, TValue?>(
                    ConvertKey(entry.Key),
                    ConvertValue(entry.Value))));
    }

    public Task<bool> Delete(TKey key)
    {
        return Delete(ConvertKey(key));
    }

    public Task<TValue?> Get(TKey key)
    {
        return Get(ConvertKey(key))
            .ContinueWith(task => ConvertValue(task.Result));
    }

    public Task<TValue?[]> Get(params TKey[] keys)
    {
        return Get(keys.Select(ConvertKey).ToArray())
            .ContinueWith(task => task.Result
                .Select(ConvertValue)
                .ToArray());
    }

    public Task<TValue?> GetDelete(TKey key)
    {
        return GetDelete(ConvertKey(key))
            .ContinueWith(task => ConvertValue(task.Result));
    }

    public Task Set(TKey key, TValue value)
    {
        return Set(ConvertKey(key), ConvertValue(value));
    }

    public Task<bool> Exists(TKey key)
    {
        return Exists(ConvertKey(key));
    }
}
