using CardboardBox;
using CardboardBox.Redis;
using CardboardBox.Redis.TestCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/*
To run this project: 
Add the "Redis:Connection" environment variable with your redis connection string
Run the project with the "redis-test" parameter in the CLI arguments.

For more information about how the "Cli(args, builder)" extension works, see the CardboardBox.Setup nuget package.
*/

//Setup the dependency injection container
return await new ServiceCollection()
    //Add the configuration (the redis connection string is fetched from environment variables)
    .AddConfig(c =>
    {
        c.AddFile("appsettings.json")
         .AddCommandLine(args)
         .AddEnvironmentVariables();
    })
    //Add serilog for logging (extension is in the CardboardBox.Setup package)
    .AddSerilog()
    //Add the redis services, getting the configuration from the appsettings.json file and the environment variables
    .AddRedis()
    //Add the CLI verb (see: CommandLineParser package, and the CardboardBox.Setup package)
    .Cli(args, c =>
    {
        c.Add<TestVerb, TestVerbOptions>();
    });