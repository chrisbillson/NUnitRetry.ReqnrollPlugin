using NUnit.Framework;
using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace NRetry.Tests.Steps
{
    [Binding]
    public class RetryTestSteps
    {
        private readonly Support.Configuration _configuration;
        private readonly ISpecFlowOutputHelper _outputHelper;

        private static int _retryCount = 0;

        public RetryTestSteps(Support.Configuration configuration, ISpecFlowOutputHelper outputHelper)
        {
            _configuration = configuration;
            _outputHelper = outputHelper;
        }

        [When(@"I increment the default retry count")]
        public void WhenIIncrementTheDefaultRetryCount()
        {
            _retryCount++;
            _outputHelper.WriteLine($"[Retry Count]: {_retryCount}");
        }
        
        [When(@"I increment the retry count")]
        public void WhenIIncrementTheRetryCount()
        {
            _retryCount++;
            _outputHelper.WriteLine($"[Retry Count]: {_retryCount}");
        }
        
        [Then(@"the retry result should be (.*)")]
        public void ThenTheRetryResultShouldBe(int expected)
        {
            Assert.AreEqual(expected, _retryCount);
        }

        [Then(@"the retry result should be equal to config")]
        public void ThenTheRetryResultShouldBeEqualToConfig()
        {
            Assert.AreEqual(_configuration.MaxRetries, _retryCount);
        }

        [Then(@"the retry result should be equal to 1 or to config value")]
        public void ThenTheRetryResultShouldBeEqualToOneOr()
        {
            if (_configuration.ApplyGlobally)
                Assert.AreEqual(_configuration.MaxRetries, _retryCount);
            else
                Assert.AreEqual(1, _retryCount);
        }

        [When(@"I increment the no retry count")]
        public void WhenIIncrementTheNoRetryCount()
        {
            
        }

    }
}
