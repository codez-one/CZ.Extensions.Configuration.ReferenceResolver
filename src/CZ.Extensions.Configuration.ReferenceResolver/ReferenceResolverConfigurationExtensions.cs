namespace CZ.Extensions.Configuration.ReferenceResolver
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Memory;

    /// <summary>
    /// IConfigurationBuilder extension methods for the reverence resolver configuration provider.
    /// </summary>
    public static class ReferenceResolverConfigurationExtensions
    {
        /// <summary>
        /// Adds reverence resolving to <paramref name="configurationBuilder"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configuration">The source <see cref="IConfiguration"/> to use.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddReferenceResolver(this IConfigurationBuilder configurationBuilder, IConfiguration configuration)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var resolvedConfiguration = new Dictionary<string, string>();
            foreach (var config in configuration.AsEnumerable())
            {
                if (ReferenceResolver.TryResolveReferences(config, configuration, out var result))
                {
                    if(result.HasReferences)
                    {
                        resolvedConfiguration.Add(result.ConfigurationPair.Key, result.ConfigurationPair.Value);
                    }
                }
                else
                {
                    var errorMessageBuilder = new StringBuilder();
                    _ = errorMessageBuilder.AppendLine("Failed to resolve the following configuration value references:");
                    foreach(var errorMessage in result.ErrorMessages)
                    {
                        _ = errorMessageBuilder.AppendLine(errorMessage);
                    }

                    throw new ArgumentException(errorMessageBuilder.ToString(), nameof(configuration));
                }
            }

            return configurationBuilder.Add(new MemoryConfigurationSource() { InitialData = resolvedConfiguration });
        }
    }
}
