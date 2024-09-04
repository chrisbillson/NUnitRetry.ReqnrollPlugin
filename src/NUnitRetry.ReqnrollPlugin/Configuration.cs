using Reqnroll.Configuration;
using SpecFlow.Internal.Json;
using System.IO;

namespace NUnitRetry.ReqnrollPlugin
{
    // Class which holds configuration from specflow.json
    // Utilizes SpecFlow.Internal.Json package which is a simple JSON parser
    public class Configuration
    {
        public int MaxRetries;
        public bool ApplyGlobally;

        private readonly IReqnrollJsonLocator _specFlowJsonLocator;

        public Configuration(IReqnrollJsonLocator specFlowJsonLocator)
        {
            _specFlowJsonLocator = specFlowJsonLocator;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            string specFlowJsonFile;
            if (File.Exists(_specFlowJsonLocator.GetReqnrollJsonFilePath()))
            {
                specFlowJsonFile = File.ReadAllText(_specFlowJsonLocator.GetReqnrollJsonFilePath());
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
