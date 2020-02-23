namespace CZ.Extensions.Configuration.ReferenceResolver.Samples.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Extend the default configuration provider
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Let's add some values as an example
                    var dict = new Dictionary<string, string>
                    {
                        {"MemoryMessage", "Hello from memory!"}
                    };

                    config.AddInMemoryCollection(dict);

                    // We have to build the current state of the configuration to use it as a source for the resolver.
                    var buildConfig = config.Build();

                    // Add the resolver to the CobfigurationBuilder with the actual build configuration.
                    // This will resolve all references in the actual configuration.
                    // Configuration sources added after this will not be resolved!
                    config.AddReferenceResolver(buildConfig);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
