using NUnit.Framework;
using Reqnroll;

namespace NUnitRetry.ReqnrollPlugin.NoConfig.Tests;

[Binding]
public class RetryTestSteps
{
    /// <summary>
    /// Gets the total number of scenario attempts.
    /// Adjusts for NUnit's <see cref="TestContext.CurrentContext.CurrentRepeatCount"/> 
    /// by adding 1 to include the initial attempt.
    /// </summary>
    private int TotalScenarioAttempts => TestContext.CurrentContext.CurrentRepeatCount + 1;

    [Then("the number of retries should be equal to {int}")]
    public void ThenTheNumberOfRetriesShouldBeEqualTo(int expected)
    {
        Assert.That(TotalScenarioAttempts, Is.EqualTo(expected));
    }

    [Then("the test should pass without retries")]
    public void ThenTheTestShouldPassWithoutRetries()
    {
        Assert.That(TotalScenarioAttempts, Is.EqualTo(1));
    }

}
