namespace CardboardBox.Redis;

/// <summary>
/// Represents the results of a redis subscription
/// </summary>
/// <param name="Channel">The channel the message came from</param>
/// <param name="Value">The message that was received</param>
public record class RedisSub(RedisChannel Channel, RedisValue Value);


/// <summary>
/// Represents the results of a redis subscription
/// </summary>
/// <param name="Channel">The channel the message came from</param>
/// <param name="Value">The message that was received</param>
public record class RedisSub<T>(RedisChannel Channel, T? Value);
