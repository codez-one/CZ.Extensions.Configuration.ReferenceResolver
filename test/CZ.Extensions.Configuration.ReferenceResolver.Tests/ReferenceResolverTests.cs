namespace CZ.Extensions.Configuration.ReferenceResolver.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class ReferenceResolverTests
    {

        private Dictionary<string, string> ConfigurationDataSource { get; set; }

        public ReferenceResolverTests() => this.ConfigurationDataSource = new Dictionary<string, string>()
            {
                { "JobClientSecret","hdasfdhajkdbgsbd23z2167tewd" },
                { "KeyVaultName", "peter" },
                { "HostBaseUrl", "https://myservice.com" },
                { "ApplicationId", "3E9DA02C-CE36-4598-8303-CF76A01B5579" }
            };

        [DataTestMethod]
        [DataRow(
            "Username",
            "Max Mustermann",
            "Max Mustermann",
            true,
            DisplayName = "No reference exist"
        )]
        [DataRow(
            "Aad:ClientSecret",
            "{WrongKeyword[JobClientSecret]}",
            "{WrongKeyword[JobClientSecret]}",
            true,
            DisplayName = "Wrong keyword used"
        )]
        [DataRow(
            "Aad:ClientSecret",
            "{References[JobClientSecret]@ Password for OAuth2 client}",
            "hdasfdhajkdbgsbd23z2167tewd",
            true,
            DisplayName = "Reference resolved for simple string"
        )]
        [DataRow(
            "Akv:Url",
            "https://{References[KeyVaultName]@ Name for key vault}.vault.azure.net/",
            "https://peter.vault.azure.net/",
            true,
            DisplayName = "Reference resolved for complex string"
        )]
        [DataRow(
            "BackendUrl",
            "{References[HostBaseUrl]@ Host base url}/applications/{References[ApplicationId]@ id of application to modify}/",
            "https://myservice.com/applications/3E9DA02C-CE36-4598-8303-CF76A01B5579/",
            true,
            DisplayName = "Multiple references resolved for complex string"
        )]
        [DataRow(
            "Aad:ClientSecret",
            "{References[JobClientSecret]}",
            "hdasfdhajkdbgsbd23z2167tewd",
            true,
            DisplayName = "Metadata is optional"
        )]
        [DataRow(
            "Logging:LogLevel:Default",
            "{References[Unicorn]@ no unicorns}",
            "",
            false,
            DisplayName = "Referenced value not found"
        )]
        [DataRow(
            "BackendUrl",
            "{References[NotExistingKey]@ Host base url}/applications/{References[ApplicationId]@ id of application to modify}/",
            "",
            false,
            DisplayName = "Failed to resolve some references for complex string"
        )]
        public void Test_TryResolveReferences_BehavesAsExpected(
            string configurationKey,
            string configurationValue,
            string expectedConfigurationValue,
            bool expectedHasReferenceValue
            )
        {
            ////////////////////
            /////// Setup //////
            ////////////////////

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(this.ConfigurationDataSource)
                .Build();

            var originalConfiguration = new KeyValuePair<string, string>(configurationKey, configurationValue);

            ////////////////////
            /////// Test ///////
            ////////////////////

            var actualHasReferenceValue = ReferenceResolver.TryResolveReferences(originalConfiguration, configuration, out var actualResult);

            /////////////////////
            /////// Assert //////
            /////////////////////

            Assert.IsTrue(expectedConfigurationValue.Equals(actualResult.ConfigurationPair.Value, StringComparison.Ordinal),
                          $"Expected configuration '{expectedConfigurationValue}' doesn't macth actual configuration value '{actualResult.ConfigurationPair.Value}'.");

            Assert.IsTrue(expectedHasReferenceValue == actualHasReferenceValue,
                          actualHasReferenceValue ? "Expected to find reference." : "Expected no reference.");
        }
    }
}
