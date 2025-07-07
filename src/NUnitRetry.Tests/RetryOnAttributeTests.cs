using NUnit.Framework;
using System;

namespace RetryOnException.NUnit.Tests;

public class NRetryAttributeTests
{
    private const int MaxRetryCount = 3;

    /// <summary>
    /// Gets the total number of scenario attempts.
    /// Adjusts for NUnit's <see cref="NUnit.Framework.TestContext.CurrentContext.CurrentRepeatCount"/> 
    /// by adding 1 to include the initial attempt.
    /// </summary>
    private int TotalScenarioAttempts => TestContext.CurrentContext.CurrentRepeatCount + 1;

    [Test, NRetry(MaxRetryCount)]
    public void ShouldPassWithoutRetries()
    {
        Assert.That(TotalScenarioAttempts, Is.EqualTo(1));
    }

    [Test, NRetry(MaxRetryCount)]
    public void ShouldRetry()
    {
        if (TotalScenarioAttempts < MaxRetryCount)
        {
            throw new ApplicationException();
        }
            
        Assert.That(TotalScenarioAttempts, Is.EqualTo(MaxRetryCount));
    }
}