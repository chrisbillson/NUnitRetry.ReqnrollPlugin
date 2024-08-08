using NUnitRetry.Tests.Steps;
using TechTalk.SpecFlow;

namespace NUnitRetry.Tests.Hooks
{
    [Binding]
    public class AfterScenario
    {
        [AfterScenario]
        public void ScenarioTeardown(ScenarioContext scenarioContext)
        {
            if (scenarioContext.TestError is null)
            {
                RetryTestSteps.RetryCount = 0;
            }
        }
    }
}
