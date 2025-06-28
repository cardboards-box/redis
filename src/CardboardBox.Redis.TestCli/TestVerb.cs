using CommandLine;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace CardboardBox.Redis.TestCli;

[Verb("redis-test")]
public class TestVerbOptions { }

public class TestVerb(
    IRedisService _redis,
    ILogger<TestVerb> _logger) : IVerb<TestVerbOptions>
{
    /// <summary>
    /// Test pub/sub with raw strings
    /// </summary>
    /// <returns></returns>
    public async Task TestRawPubSub()
    {
        const string CHANNEL = "test-event";

        //Setup an observable to list for events
        _logger.LogInformation("Setting up observer for raw pub/sub!");
        using var sub = (await _redis.Observe(CHANNEL))
            .Subscribe(x => _logger.LogInformation("Found event: {event}", x));

        //Publish 10 events to the channel
        for (var i = 0; i < 10; i++)
        {
            _logger.LogInformation("Publishing event: {event}", "test-" + i);
            await _redis.Publish(CHANNEL, "test-" + i);
            await Task.Delay(500);
        }

        _logger.LogInformation("Waiting for remaining events");
        await Task.Delay(100);
    }

    /// <summary>
    /// Test pub/sub with complex objects
    /// </summary>
    /// <returns></returns>
    public async Task TestJsonPubSub()
    {
        const string CHANNEL = "test-complex-event";

        _logger.LogInformation("Setting up observer for complex/json pub/sub!");
        using var sub = (await _redis.Observe<SomeUser>(CHANNEL))
            .Subscribe(x => _logger.LogInformation("Found user: {Id} - {Name} ({Age})", x?.Id, x?.Name, x?.Age));

        var rnd = new Random();
        for (var i = 0; i < 10; i++)
        {
            var user = new SomeUser(i, "Test User - " + i, rnd.Next(15, 45));
            _logger.LogInformation("Publishing user: {Id} - {Name} ({Age})", user.Id, user.Name, user.Age);
            await _redis.Publish(CHANNEL, user);
            await Task.Delay(500);
        }

        _logger.LogInformation("Waiting for remaining events");
        await Task.Delay(100);
    }

    /// <summary>
    /// Test setting, fetching and deleting some raw data
    /// </summary>
    /// <returns></returns>
    public async Task TestRawStore()
    {
        const string KEY = "test-data";

        _logger.LogInformation("Setting up some raw test data");
        var worked = await _redis.Set(KEY, "Hello World!");
        _logger.LogInformation("Set Redis Key: {key} - Worked: {worked}", KEY, worked);

        var value = await _redis.Get(KEY);
        _logger.LogInformation("Got Redis Key: {key} - Value: {value}", KEY, value);
        
        worked = await _redis.Delete(KEY);
        _logger.LogInformation("Deleted Redis Key: {key} - Worked: {worked}", KEY, worked);
    }

    /// <summary>
    /// Test setting, fetching, and deleting some complex data
    /// </summary>
    /// <returns></returns>
    public async Task TestJsonStore()
    {
        const string KEY = "test-complex-data";
        var user = new SomeUser(99, "Hello world", 26);

        _logger.LogInformation("Setting up some complex test data");
        var worked = await _redis.Set(KEY, user);
        _logger.LogInformation("Set Redis Key: {key} - Worked: {worked}", KEY, worked);

        user = await _redis.Get<SomeUser>(KEY);
        _logger.LogInformation("Got Redis Key: {key} - Value: {Id} - {Name} ({Age})", KEY, user?.Id, user?.Name, user?.Age);

        worked = await _redis.Delete(KEY);
        _logger.LogInformation("Deleted Redis Key: {key} - Worked: {worked}", KEY, worked);
    }

    /// <summary>
    /// Test the hash-set functionality of redis
    /// </summary>
    /// <returns></returns>
    public async Task TestHashSet()
    {
        const string KEY = "test-hash-set";
        var hashSet = _redis.HashSet<int, SomeUser>(KEY);

        _logger.LogInformation("Setting up some raw test data");
        await hashSet.Set(1, new SomeUser(1, "User 1", 18));
        await hashSet.Set(2, new SomeUser(2, "User 2", 21));
        await hashSet.Set(3, new SomeUser(3, "User 3", 50));
        _logger.LogInformation("Set Redis HashSet Key: {key} - Count: {count}", KEY, await hashSet.Count());

        var user = await hashSet.Get(1);
        _logger.LogInformation("Got Redis HashSet Key: {key} - User 1: {Id} - {Name} ({Age})", KEY, user?.Id, user?.Name, user?.Age);
        user = await hashSet.Get(2);
        _logger.LogInformation("Got Redis HashSet Key: {key} - User 2: {Id} - {Name} ({Age})", KEY, user?.Id, user?.Name, user?.Age);
        user = await hashSet.Get(3);
        _logger.LogInformation("Got Redis HashSet Key: {key} - User 3: {Id} - {Name} ({Age})", KEY, user?.Id, user?.Name, user?.Age);
        await hashSet.Delete(2);
        _logger.LogInformation("Deleted User 2 from Redis HashSet Key: {key} - Count: {count}", KEY, await hashSet.Count());

        var all = await hashSet.All();
        foreach (var kvp in all)
            _logger.LogInformation("User: {Id} - {Name} ({Age})", kvp.Key, kvp.Value?.Name, kvp.Value?.Age);

        await hashSet.Clear();
        _logger.LogInformation("Cleared Redis HashSet Key: {key} - Count: {count}", KEY, await hashSet.Count());
    }

    /// <summary>
    /// Run all of the tests
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Test()
    {
        await TestRawStore();
        await TestJsonStore();
        await TestRawPubSub();
        await TestJsonPubSub();
        await TestHashSet();
        return true;
    }

    /// <summary>
    /// Execute the given verb and return the exit code (this is the entry method for this verb)
    /// </summary>
    /// <param name="_">We don't really care about options for this test</param>
    /// <returns></returns>
    public async Task<int> Run(TestVerbOptions _, CancellationToken __)
    {
        try
        {
            return await Test() ? 0 : 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while testing redis");
            return 1;
        }
    }
}

/// <summary>
/// Just a random test class for complex data tests
/// </summary>
/// <param name="Id">Some random user ID</param>
/// <param name="Name">Some random user name</param>
/// <param name="Age">Some random user age</param>
public record class SomeUser(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("age")] int Age);