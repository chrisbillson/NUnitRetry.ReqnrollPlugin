using Reqnroll.Configuration;
using SpecFlow.Internal.Json;
using System.IO;

namespace NUnitRetry.Tests.Support
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
                throw new FileNotFoundException("specflow.json is missing!");
            }
            var jsonConfig = specFlowJsonFile.FromJson<JsonConfig.JsonConfig>();


            MaxRetries = jsonConfig.NRetrySettings.MaxRetries;
            ApplyGlobally = jsonConfig.NRetrySettings.ApplyGlobally;
        }
    }
}
