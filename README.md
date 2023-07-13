# CardboardBox.Redis
Provides a simple wrapper around the `StackExchange.Redis` library to make it easier to use.

## Installation
You can install the nuget package from within Visual Studio or the Package Manager Console. 
The project targets .net standard 2.1.

```
PM> Install-Package CardboardBox.Redis
```

## Dependencies
This library depends on the `IJsonService` provided in `CardboardBox.Json` and will need to be provided in the dependency injection setup.
By default the library will use the `System.Text.Json` library for serialization and deserialization.
This can be changed by providing a custom implementation of `IJsonService` in the dependency injection setup.
There is also a default implementation of `IJsonService` provided in `CardboardBox.Json` that uses `Newtonsoft.Json` for serialization and deserialization.

You will need to use one of the following to register the `IJsonService` in the dependency injection setup:
```csharp
using CardboardBox.Json;
using Microsoft.Extensions.DependencyInjection;
...
var services = new ServiceCollection();
...
services.AddJson(); //Uses System.Text.Json (You can pass in a custom JsonSerializerOptions instance)

services.AddJson<NewtonsoftJsonService>(); //Uses Newtonsoft.Json

//Alternatively you can provide your own implementation of IJsonService
services.AddJson<MyCustomJsonService>();
```


## Setup
The library is designed to be used with dependency injection.
There are a few ways to provide the configuration for how to connect to redis.
They are as follows

### From `IConfiguration`
You can provide the configuration from an injected `IConfiguration` instance using the following:

```csharp
using CardboardBox.Redis;
using Microsoft.Extensions.DependencyInjection;
...
var services = new ServiceCollection();
...

services.AddRedis();
```

This will load the following values from the configuration instance:
* `Redis:ConnectionString` - This is the standard connection string for attaching to a redis instance.
* `Redis:Prefix:Data` - This is the optional prefix that will be applied to all keys when interacting with redis.
* `Redis:Prefix:Events` - This is the optional prefix that will be applied to all keys when interacting with pub/subs within redis.

### From a static configuration
You can provide the configuration directly in the dependency injection statement:

```csharp
using CardboardBox.Redis;
using Microsoft.Extensions.DependencyInjection;
...
var services = new ServiceCollection();
...

//Register with just the connection string
services.AddRedis("localhost:6379,password=S0meP4ssw0rd!");

//Register with StackExchange.Redis.ConfigurationOptions
using StackExchange.Redis;
...
var opts = new ConfigurationOptions();
services.AddRedis(opts);
```

### From a custom `IRedisConfig` implementation
You can provide your own implementation of `IRedisConfig` to provide the configuration:

```csharp
using CardboardBox.Redis;
using Microsoft.Extensions.DependencyInjection;
...
var services = new ServiceCollection();
...
//As a transient
services.AddRedis<MyCustomRedisConfig>();
//As a singleton
services.AddRedis(new MyCustomRedisConfig());
```

## Usage
The library provides 3 interfaces for the interaction: 
* `IRedisConfig` - which contains the configuration necessary for connecting and interfacing with redis.
* `IRedisConnection` - which maintains the instance of the lazy loaded `ConnectionMultiplexer` necessary for interacting with redis (This is a singleton by default as there [should only be 1 instance per application](https://developer.redis.com/develop/dotnet/#step-3-initialize-the-connectionmultiplexer)).
* `IRedisService` - which provides some handy methods for interacting with redis and pub/subs.

The application takes an asynconous approach to interacting with redis, so all methods are asyncronous and return some form of `Task`.
By default all of the interactions with redis have the appropriate prefix applied to the key automatically. 
These prefixes are gotten from the `IRedisConfig` instance and can be changed at runtime.

Some common methods for interacting with redis are as follows:
* `Task<string?> IRedisService.Get(string key)` - Gets the value of a key from redis or null if the key does not exist.
* `Task<T?> IRedisService.Get<T>(string key)` - Gets the value of a key from redis or null if the key does not exist, and deserializes it using the `IJsonService` instance.
* `Task<bool> IRedisService.Set(string key, string value)` - Sets the value of a key in redis.
* `Task<bool> IRedisService.Set<T>(string key, T value)` - Sets the value of a key in redis, serializing it using the `IJsonService` instance.
* `Task<bool> IRedisService.Delete(string key)` - Deletes the value of a key in redis.

There are also some helper classes used for interacting with lists:
* `IRedisList IRedisService.List(string key)` - Gets an instance of the `IRedisList` helper class scoping it to the given prefix.
* `IRedisList<T> IRedisService.List<T>(string key)` - Gets an instance of the `IRedisList` helper class scoping it to the given prefix and handles the serialization using the `IJsonService` instance.

There are also various methods for handling pub/sub either directly or using `System.Reactive` observables and subjects.
You can also interact with the underlying `IDatabase` and `ISubscriber` instances directly if you need to: `Task<IDatabase> IRedisService.GetSubscriber()` and `Task<ISubscriber> IRedisService.GetSubscriber()`.
These do not handle prefixes automatically and by-pass any data checks the `IRedisService` instance would normally do.

The intellisense/XML documentation comments are included in the Nuget package and should provide adequate information on how to use the library. 
Feel free to open an issue if you have any questions or suggestions.
