using Cackle.ConsoleApp;
using SimpleExample;

// Create a host to handle services, configuration, and execution
var host = CommandHostBuilder.Create();

// Register our command with its argument specification
host.RegisterCommand<HelloWorld, HelloWorldArgs>();

// Finally, build the host and run it
return host.Build().Run(args);