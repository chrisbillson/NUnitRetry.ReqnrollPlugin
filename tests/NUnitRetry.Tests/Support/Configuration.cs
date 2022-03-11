using SpecFlow.Internal.Json;
using System.IO;
using TechTalk.SpecFlow.Configuration;

namespace NUnitRetry.Tests.Support
{
    // Class which holds configuration from specflow.json
    // Utilizes SpecFlow.Internal.Json package which is a simple JSON parser
    public class Configuration
    {
        public int MaxRetries;
        public bool ApplyGlobally;

        private readonly ISpecFlowJsonLocator _specFlowJsonLocator;

        public Configuration(ISpecFlowJsonLocator specFlowJsonLocator)
        {
            _specFlowJsonLocator = specFlowJsonLocator;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            string specFlowJsonFile;
            if (File.Exists(_specFlowJsonLocator.GetSpecFlowJsonFilePath()))
            {
                specFlowJsonFile = File.ReadAllText(_specFlowJsonLocator.GetSpecFlowJsonFilePath());
            }
            else
            {
                throw new FileNotFoundException("specflow.json is missing!");
            }
            var jsonConfig = specFlowJsonFile.FromJson<JsonConfig.JsonConfig>();


            MaxRetries = jsonConfig.NRetrySettings.MaxRetries;
            ApplyGlobally = jsonConfig.NRetrySettings.ApplyGlobally;
        }
    }
}
