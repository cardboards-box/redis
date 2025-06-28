namespace CardboardBox.Redis;

/// <summary>
/// Provides access to redis list operations of a given type.
/// Note: This is a transient dummy object, and instances shouldn't be retained / persisted. Just create a new one.
/// </summary>
/// <typeparam name="T">The type of items in the list</typeparam>
public interface IRedisList<T>
{
    /// <summary>
    /// The key of the list in redis
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Adds the given elements to the start of the list
    /// </summary>
    /// <param name="values">The data to prepend to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    Task<long> Prepend(params T[] values);

    /// <summary>
    /// Adds the given elements to the end of the list
    /// </summary>
    /// <param name="values">The data to append to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    Task<long> Append(params T[] values);

    /// <summary>
    /// Gets the element at the given position in the list
    /// </summary>
    /// <param name="index">The index</param>
    /// <returns>The element at the given position</returns>
    Task<T?> At(long index);

    /// <summary>
    /// Sets the element at the given position in the list
    /// </summary>
    /// <param name="index">The position of the element</param>
    /// <param name="value">The value to set</param>
    Task Set(long index, T value);

    /// <summary>
    /// Gets the length of the list
    /// </summary>
    /// <returns>The count of the elements in the list</returns>
    Task<long> Length();

    /// <summary>
    /// Removes and returns the first item in the list
    /// </summary>
    /// <returns>The first element in the list or null if there were no elements in the list</returns>
    Task<T?> Pop();

    /// <summary>
    /// Removes and returns the given number of elements from the beginning of the list
    /// </summary>
    /// <param name="count">The number of elements</param>
    /// <returns>The given number of elements from the beginning of the list</returns>
    Task<T[]> Pop(long count);

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    Task<T?> PopTail();

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    Task<T[]> PopTail(long count);

    /// <summary>
    /// Gets the given range of elements from the list
    /// Note: Providing no arguments returns the entire list.
    /// </summary>
    /// <param name="start">The index of the first item to return</param>
    /// <param name="end">The index of the last item in the range</param>
    /// <returns>The elements of the given range</returns>
    Task<T[]> Range(long start = 0, long end = -1);

    /// <summary>
    /// Gets all of the items in the list
    /// </summary>
    /// <returns>All of the items in the list</returns>
    Task<T[]> All();

    /// <summary>
    /// Removes the elements that match the given value.
    /// The count dictates the direction and count of the items to remove.
    /// count less than 0 removes from the end of the list
    /// count greater than 0 removes from the start of the list
    /// count equals 0 removes all matching elements in the list
    /// </summary>
    /// <param name="value">The value to match against</param>
    /// <param name="count">Dictates the direction and count of the items to remove.</param>
    /// <returns>The number of elements removed by the operation</returns>
    Task<long> Remove(T value, long count = 0);

    /// <summary>
    /// Trim an existing list so that it will contain only the specified range of elements specified.
    /// Both start and stop are zero-based indexes, where 0 is the first element of the list (the head), 1 the next element and so on.
    /// For example: <c>LTRIM foobar 0 2</c> will modify the list stored at foobar so that only the first three elements of the list will remain.
    /// start and end can also be negative numbers indicating offsets from the end of the list, where -1 is the last element of the list, -2 the penultimate element and so on.
    /// </summary>
    /// <param name="start">The start index of the list to trim to.</param>
    /// <param name="stop">The end index of the list to trim to.</param>
    Task Trim(long start, long stop);
}

/// <summary>
/// Provides access to redis list operations of a given type.
/// Note: This is a transient dummy object, and instances shouldn't be retained / persisted. Just create a new one.
/// </summary>
/// <typeparam name="T">The type of items in the list</typeparam>
internal class RedisList<T>(
    IRedisService _redis, 
    string prefix, 
    IRedisJsonService _json) : IRedisList<T>
{
    private readonly IRedisList _list = new RedisList(_redis, prefix);

    /// <summary>
    /// The key of the list in redis
    /// </summary>
    public string Key => _list.Key;

    /// <summary>
    /// Adds the given elements to the start of the list
    /// </summary>
    /// <param name="values">The data to prepend to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    public Task<long> Prepend(params T[] values) => _list.Prepend(values.Select(t => _json.Serialize(t)).ToArray());

    /// <summary>
    /// Adds the given elements to the end of the list
    /// </summary>
    /// <param name="values">The data to append to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    public Task<long> Append(params T[] values) => _list.Append(values.Select(t => _json.Serialize(t)).ToArray());

    /// <summary>
    /// Gets the element at the given position in the list
    /// </summary>
    /// <param name="index">The index</param>
    /// <returns>The element at the given position</returns>
    public async Task<T?> At(long index)
    {
        return Convert(await _list.At(index));
    }

    /// <summary>
    /// Sets the element at the given position in the list
    /// </summary>
    /// <param name="index">The position of the element</param>
    /// <param name="value">The value to set</param>
    /// <returns></returns>
    public Task Set(long index, T value) => _list.Set(index, _json.Serialize(value));

    /// <summary>
    /// Gets the length of the list
    /// </summary>
    /// <returns>The count of the elements in the list</returns>
    public Task<long> Length() => _list.Length();

    /// <summary>
    /// Removes and returns the first item in the list
    /// </summary>
    /// <returns>The first element in the list or null if there were no elements in the list</returns>
    public async Task<T?> Pop()
    {
        return Convert(await _list.Pop());
    }

    /// <summary>
    /// Removes and returns the given number of elements from the beginning of the list
    /// </summary>
    /// <param name="count">The number of elements</param>
    /// <returns>The given number of elements from the beginning of the list</returns>
    public async Task<T[]> Pop(long count)
    {
        return Convert(await _list.Pop(count));
    }

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    public async Task<T?> PopTail()
    {
        return Convert(await _list.PopTail());
    }

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    public async Task<T[]> PopTail(long count)
    {
        return Convert(await _list.PopTail(count));
    }

    /// <summary>
    /// Gets the given range of elements from the list
    /// Note: Providing no arguments returns the entire list.
    /// </summary>
    /// <param name="start">The index of the first item to return</param>
    /// <param name="end">The index of the last item in the range</param>
    /// <returns>The elements of the given range</returns>
    public async Task<T[]> Range(long start = 0, long end = -1)
    {
        return Convert(await _list.Range(start, end));
    }

    /// <summary>
    /// Gets all of the items in the list
    /// </summary>
    /// <returns>All of the items in the list</returns>
    public Task<T[]> All() => Range(0, -1);

    /// <summary>
    /// Removes the elements that match the given value.
    /// The count dictates the direction and count of the items to remove.
    /// count less than 0 removes from the end of the list
    /// count greater than 0 removes from the start of the list
    /// count equals 0 removes all matching elements in the list
    /// </summary>
    /// <param name="value">The value to match against</param>
    /// <param name="count">Dictates the direction and count of the items to remove.</param>
    /// <returns>The number of elements removed by the operation</returns>
    public Task<long> Remove(T value, long count = 0)
    {
        return _list.Remove(_json.Serialize(value), count);
    }

    /// <summary>
    /// Trim an existing list so that it will contain only the specified range of elements specified.
    /// Both start and stop are zero-based indexes, where 0 is the first element of the list (the head), 1 the next element and so on.
    /// For example: <c>LTRIM foobar 0 2</c> will modify the list stored at foobar so that only the first three elements of the list will remain.
    /// start and end can also be negative numbers indicating offsets from the end of the list, where -1 is the last element of the list, -2 the penultimate element and so on.
    /// </summary>
    /// <param name="start">The start index of the list to trim to.</param>
    /// <param name="stop">The end index of the list to trim to.</param>
    /// <returns></returns>
    public Task Trim(long start, long stop) => _list.Trim(start, stop);

    /// <summary>
    /// Converts between a <see cref="RedisValue"/> and the current list type
    /// </summary>
    /// <param name="value">The value to convert from</param>
    /// <returns>The deserialized value</returns>
    public T? Convert(RedisValue value)
    {
        if (value.IsNullOrEmpty) return default;
        return _json.Deserialize<T>(value.ToString());
    }

    /// <summary>
    /// Converts between a collection of <see cref="RedisValue"/>s and the current list type, filtering out any null values.
    /// </summary>
    /// <param name="items">The values to convert from</param>
    /// <returns>The deserialized values</returns>
    public T[] Convert(RedisValue[] items)
    {
        return items
            .Where(t => !t.IsNull)
            .Select(t => _json.Deserialize<T>(t.ToString()))
            .Where(t => t != null)
            .Select(t => t!)
            .ToArray();
    }
}