namespace CZ.Extensions.Configuration.ReferenceResolver
{
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class ReferenceResolver
    {
        public static bool TryResolveReferences(
            KeyValuePair<string,string> originalConfigurationPair,
            IConfiguration configuration,
            out ReferenceResolverResult result)
        {
            var hasReferences = false;
            var hasResolvedAllReferences = true;
            var resolvedConfigurationValue = originalConfigurationPair.Value;
            var errorMessage = new List<string>();

            if(!string.IsNullOrEmpty(originalConfigurationPair.Value))
            {
                // Sample 1: https://{References[ClientId]@this-is-a-comment}/applicationid/{References[ApplicationId]@Source:SharedKeyVault}
                // Sample 2: https://{References[ClientId]}/applicationid/{References[ApplicationId]@Source:SharedKeyVault}
                var configurationReferenceRegex = new Regex(@"\{References\[(?<ConfigKey>[^\]]+)\](?:@(?<Metadata>[^}]+))?\}");

                var matches = configurationReferenceRegex.Matches(originalConfigurationPair.Value);
                if(matches.Cast<Match>().Any())
                {
                    hasReferences = true;

                    foreach (Match match in matches)
                    {
                        var referencedConfigKey = match.Groups["ConfigKey"].Value;
                        var metadata = match.Groups["Metadata"]?.Value ?? string.Empty;

                        var resolvedValue = configuration[referencedConfigKey];

                        if (string.IsNullOrEmpty(resolvedValue))
                        {
                            errorMessage.Add($"The value for configuration key '{originalConfigurationPair.Key}' contains a reference to a non existing configuration key '{referencedConfigKey}'.");
                            hasResolvedAllReferences = false;
                        }
                        else
                        {
                            resolvedConfigurationValue = resolvedConfigurationValue
                                .Replace(
                                    string.Concat(
                                        $"{{References[{referencedConfigKey}]",
                                        !string.IsNullOrWhiteSpace(metadata) ? $"@{metadata}}}" : "}"
                                    ),
                                    resolvedValue
                                );
                        }
                    }
                }
            }

            if(errorMessage.Any())
            {
                result = new ReferenceResolverResult(errorMessage);
            }
            else
            {
                result = new ReferenceResolverResult(
                    new KeyValuePair<string, string>(
                        originalConfigurationPair.Key,
                        resolvedConfigurationValue
                    ),
                    hasReferences
                );
            }

            return hasResolvedAllReferences;
        }
    }
}
