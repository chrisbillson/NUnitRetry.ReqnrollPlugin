using NUnit.Framework;
using NUnitRetry.ReqnrollPlugin;
using Reqnroll;

namespace NUnitRetry.Tests.Steps;

[Binding]
public class RetryTestSteps
{
    private readonly RetryConfiguration _configuration;

    /// <summary>
    /// Gets the total number of scenario attempts.
    /// Adjusts for NUnit's <see cref="NUnit.Framework.TestContext.CurrentContext.CurrentRepeatCount"/> 
    /// by adding 1 to include the initial attempt.
    /// </summary>
    private int TotalScenarioAttempts => TestContext.CurrentContext.CurrentRepeatCount + 1;

    public RetryTestSteps(RetryConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Then("assertion failures should pass after {int} attempts")]
    public void ThenAssertionFailuresShouldPassAfterRetries(int expectedRetries)
    {
        Assert.That(TotalScenarioAttempts, Is.EqualTo(expectedRetries));
    }

    [Then("the number of retries should be equal to value from config")]
    public void ThenTheNumberOfRetriesShouldBeEqualToValueFromConfig()
    {
        Assert.That(TotalScenarioAttempts, Is.EqualTo(_configuration.MaxRetries));
    }

    [Then("the number of retries should be equal to {int}")]
    public void ThenTheNumberOfRetriesShouldBeEqualTo(int expected)
    {
        Assert.That(TotalScenarioAttempts, Is.EqualTo(expected));
    }

    [Then("the number of retries should not be equal to value from config")]
    public void ThenTheNumberOfRetriesShouldNotBeEqualToValueFromConfig()
    {
        Assert.That(TotalScenarioAttempts, Is.Not.EqualTo(_configuration.MaxRetries));
    }

    [Then("the retry result should be {int}")]
    [Then("the retry result with failure from assertion should be {int}")]
    public void ThenTheRetryResultWithFailureFromAssertionShouldBe(int expected)
    {
        Assert.That(TotalScenarioAttempts, Is.EqualTo(expected));
    }

    [Then("exception failures should pass after {int} attempts")]
    public void ThenExceptionFailuresShouldPassAfterRetries(int expectedRetries)
    {
        if (TotalScenarioAttempts < expectedRetries)
        {
            throw new Exception("retry");
        }

        Assert.That(TotalScenarioAttempts, Is.EqualTo(expectedRetries));
    }
}
