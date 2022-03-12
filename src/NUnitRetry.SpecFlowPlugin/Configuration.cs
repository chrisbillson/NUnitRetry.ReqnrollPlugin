
using SpecFlow.Internal.Json;
using System.IO;
using TechTalk.SpecFlow.Configuration;

namespace NUnitRetry.SpecFlowPlugin
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
                throw new FileNotFoundException("specflow.json is missing! Ensure that you've provided specflow.json file to your project and added correct section. For more info proceed to the projects page: https://github.com/farum12/NUnitRetry.SpecFlowPlugin");
            }

            try
            {
                var jsonConfig = specFlowJsonFile.FromJson<JsonConfig.JsonConfig>();

                MaxRetries = jsonConfig.NRetrySettings.MaxRetries;
                ApplyGlobally = jsonConfig.NRetrySettings.ApplyGlobally;
            }
            catch
            {
                // Apply default values if specflow.json is present, but section is not added to the JSON.
                MaxRetries = 3;
                ApplyGlobally = true;
            }
            


            
        }
    }
}
