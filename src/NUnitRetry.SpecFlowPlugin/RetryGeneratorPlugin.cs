using Reqnroll.Generator.Plugins;
using Reqnroll.Generator.UnitTestProvider;
using Reqnroll.Infrastructure;
using Reqnroll.UnitTestProvider;

[assembly: GeneratorPlugin(typeof(NUnitRetry.SpecFlowPlugin.RetryGeneratorPlugin))]

namespace NUnitRetry.SpecFlowPlugin
{
    class RetryGeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            generatorPluginEvents.CustomizeDependencies += CustomiseDependencies;
        }

        // We must override TestGeneratorProvider
        private void CustomiseDependencies(object sender, CustomizeDependenciesEventArgs eventArgs)
        {
            eventArgs.ObjectContainer.RegisterTypeAs<TestGeneratorProvider, IUnitTestGeneratorProvider>();
        }
    }
}
