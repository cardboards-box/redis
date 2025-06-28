namespace CardboardBox.Redis;

/// <summary>
/// Provides access to redis list operations. 
/// Note: This is a transient dummy object, and instances shouldn't be retained / persisted. Just create a new one.
/// </summary>
public interface IRedisList
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
    Task<long> Prepend(params RedisValue[] values);

    /// <summary>
    /// Adds the given elements to the start of the list
    /// </summary>
    /// <param name="values">The data to prepend to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    Task<long> Prepend(params string[] values);

    /// <summary>
    /// Adds the given elements to the end of the list
    /// </summary>
    /// <param name="values">The data to append to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    Task<long> Append(params RedisValue[] values);

    /// <summary>
    /// Adds the given elements to the end of the list
    /// </summary>
    /// <param name="values">The data to append to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    Task<long> Append(params string[] values);

    /// <summary>
    /// Gets the element at the given position in the list
    /// </summary>
    /// <param name="index">The index</param>
    /// <returns>The element at the given position</returns>
    Task<RedisValue> At(long index);

    /// <summary>
    /// Sets the element at the given position in the list
    /// </summary>
    /// <param name="index">The position of the element</param>
    /// <param name="value">The value to set</param>
    /// <returns></returns>
    Task Set(long index, RedisValue value);

    /// <summary>
    /// Sets the element at the given position in the list
    /// </summary>
    /// <param name="index">The position of the element</param>
    /// <param name="value">The value to set</param>
    /// <returns></returns>
    Task Set(long index, string value);

    /// <summary>
    /// Gets the length of the list
    /// </summary>
    /// <returns>The count of the elements in the list</returns>
    Task<long> Length();

    /// <summary>
    /// Removes and returns the first item in the list
    /// </summary>
    /// <returns>The first element in the list</returns>
    Task<RedisValue> Pop();

    /// <summary>
    /// Removes and returns the given number of elements from the beginning of the list
    /// </summary>
    /// <param name="count">The number of elements</param>
    /// <returns>The given number of elements from the beginning of the list</returns>
    Task<RedisValue[]> Pop(long count);

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    Task<RedisValue> PopTail();

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    Task<RedisValue[]> PopTail(long count);

    /// <summary>
    /// Gets the given range of elements from the list
    /// Note: Providing no arguments returns the entire list.
    /// </summary>
    /// <param name="start">The index of the first item to return</param>
    /// <param name="end">The index of the last item in the range</param>
    /// <returns>The elements of the given range</returns>
    Task<RedisValue[]> Range(long start = 0, long end = -1);

    /// <summary>
    /// Gets all of the items in the list
    /// </summary>
    /// <returns>All of the items in the list</returns>
    Task<RedisValue[]> All();

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
    Task<long> Remove(RedisValue value, long count = 0);

    /// <summary>
    /// Trim an existing list so that it will contain only the specified range of elements specified.
    /// Both start and stop are zero-based indexes, where 0 is the first element of the list (the head), 1 the next element and so on.
    /// For example: <c>LTRIM foobar 0 2</c> will modify the list stored at foobar so that only the first three elements of the list will remain.
    /// start and end can also be negative numbers indicating offsets from the end of the list, where -1 is the last element of the list, -2 the penultimate element and so on.
    /// </summary>
    /// <param name="start">The start index of the list to trim to.</param>
    /// <param name="stop">The end index of the list to trim to.</param>
    /// <returns></returns>
    Task Trim(long start, long stop);
}

/// <summary>
/// Provides access to redis list operations. 
/// Note: This is a transient dummy object, and instances shouldn't be retained / persisted. Just create a new one.
/// </summary>
internal class RedisList(
    IRedisService _redis, 
    string _key) : IRedisList
{
    /// <summary>
    /// The key of the list in redis
    /// </summary>
    public string Key => _redis.Prefix(_key);

    /// <summary>
    /// Adds the given elements to the start of the list
    /// </summary>
    /// <param name="values">The data to prepend to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    public async Task<long> Prepend(params RedisValue[] values)
    {
        var database = await _redis.GetDatabase();
        return await database.ListLeftPushAsync(Key, values);
    }

    /// <summary>
    /// Adds the given elements to the start of the list
    /// </summary>
    /// <param name="values">The data to prepend to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    public Task<long> Prepend(params string[] values) => Prepend(values.Convert());

    /// <summary>
    /// Adds the given elements to the end of the list
    /// </summary>
    /// <param name="values">The data to append to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    public async Task<long> Append(params RedisValue[] values)
    {
        var database = await _redis.GetDatabase();
        return await database.ListRightPushAsync(Key, values);
    }

    /// <summary>
    /// Adds the given elements to the end of the list
    /// </summary>
    /// <param name="values">The data to append to the list</param>
    /// <returns>The count of the elements in the list after the operation completes</returns>
    public Task<long> Append(params string[] values) => Append(values.Convert());

    /// <summary>
    /// Gets the element at the given position in the list
    /// </summary>
    /// <param name="index">The index</param>
    /// <returns>The element at the given position</returns>
    public async Task<RedisValue> At(long index)
    {
        var database = await _redis.GetDatabase();
        return await database.ListGetByIndexAsync(Key, index);
    }

    /// <summary>
    /// Sets the element at the given position in the list
    /// </summary>
    /// <param name="index">The position of the element</param>
    /// <param name="value">The value to set</param>
    /// <returns></returns>
    public async Task Set(long index, RedisValue value)
    {
        var database = await _redis.GetDatabase();
        await database.ListSetByIndexAsync(Key, index, value);
    }

    /// <summary>
    /// Sets the element at the given position in the list
    /// </summary>
    /// <param name="index">The position of the element</param>
    /// <param name="value">The value to set</param>
    /// <returns></returns>
    public Task Set(long index, string value) => Set(index, value.Convert());

    /// <summary>
    /// Gets the length of the list
    /// </summary>
    /// <returns>The count of the elements in the list</returns>
    public async Task<long> Length()
    {
        var database = await _redis.GetDatabase();
        return await database.ListLengthAsync(Key);
    }

    /// <summary>
    /// Removes and returns the first item in the list
    /// </summary>
    /// <returns>The first element in the list</returns>
    public async Task<RedisValue> Pop()
    {
        var database = await _redis.GetDatabase();
        return await database.ListLeftPopAsync(Key);
    }

    /// <summary>
    /// Removes and returns the given number of elements from the beginning of the list
    /// </summary>
    /// <param name="count">The number of elements</param>
    /// <returns>The given number of elements from the beginning of the list</returns>
    public async Task<RedisValue[]> Pop(long count)
    {
        var database = await _redis.GetDatabase();
        return await database.ListLeftPopAsync(Key, count);
    }

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    public async Task<RedisValue> PopTail()
    {
        var database = await _redis.GetDatabase();
        return await database.ListRightPopAsync(Key);
    }

    /// <summary>
    /// Removes and returns the last item in the list
    /// </summary>
    /// <returns>The last element in the list</returns>
    public async Task<RedisValue[]> PopTail(long count)
    {
        var database = await _redis.GetDatabase();
        return await database.ListRightPopAsync(Key, count);
    }

    /// <summary>
    /// Gets the given range of elements from the list
    /// Note: Providing no arguments returns the entire list.
    /// </summary>
    /// <param name="start">The index of the first item to return</param>
    /// <param name="end">The index of the last item in the range</param>
    /// <returns>The elements of the given range</returns>
    public async Task<RedisValue[]> Range(long start = 0, long end = -1)
    {
        var database = await _redis.GetDatabase();
        return await database.ListRangeAsync(Key, start, end);
    }

    /// <summary>
    /// Gets all of the items in the list
    /// </summary>
    /// <returns>All of the items in the list</returns>
    public Task<RedisValue[]> All() => Range(0, -1);

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
    public async Task<long> Remove(RedisValue value, long count = 0)
    {
        var database = await _redis.GetDatabase();
        return await database.ListRemoveAsync(Key, value, count);
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
    public async Task Trim(long start, long stop)
    {
        var database = await _redis.GetDatabase();
        await database.ListTrimAsync(Key, start, stop);
    }
}