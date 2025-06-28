using System.Text.Json;

namespace CardboardBox.Redis;

using Json;

/// <summary>
/// Implementation for <see cref="RedisJsonService"/>
/// </summary>
public interface IRedisJsonService : IJsonService { }

internal class RedisJsonService(
    JsonSerializerOptions settings) : SystemTextJsonService(settings), IRedisJsonService
{ }
