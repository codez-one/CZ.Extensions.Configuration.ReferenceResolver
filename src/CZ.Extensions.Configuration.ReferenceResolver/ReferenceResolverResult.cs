namespace CZ.Extensions.Configuration.ReferenceResolver
{
    using System.Collections.Generic;

    public class ReferenceResolverResult
    {
        #region Properties

        public KeyValuePair<string,string> ConfigurationPair { get; private set; }

        public bool HasReferences { get; private set; }

        public IEnumerable<string> ErrorMessages { get; private set; }

        #endregion

        #region Constructor

        public ReferenceResolverResult(KeyValuePair<string, string> configurationPair, bool hasReferences)
        {
            this.ConfigurationPair = configurationPair;
            this.HasReferences = hasReferences;
            this.ErrorMessages = new List<string>() { };
        }

        public ReferenceResolverResult(IEnumerable<string> errorMessages)
        {
            this.HasReferences = true;
            this.ConfigurationPair = new KeyValuePair<string, string>(string.Empty, string.Empty);
            this.ErrorMessages = new List<string>(errorMessages);
        }

        #endregion
    }
}
