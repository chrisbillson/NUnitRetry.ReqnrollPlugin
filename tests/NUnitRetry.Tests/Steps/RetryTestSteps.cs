using NUnit.Framework;
using Reqnroll;

namespace NUnitRetry.Tests.Steps
{
    [Binding]
    public class RetryTestSteps
    {
        private readonly Support.Configuration _configuration;
        private readonly IReqnrollOutputHelper _outputHelper;

        public static int RetryCount { get; set; }

        public RetryTestSteps(Support.Configuration configuration, IReqnrollOutputHelper outputHelper)
        {
            _configuration = configuration;
            _outputHelper = outputHelper;
        }

        [When(@"I increment the default retry count")]
        public void WhenIIncrementTheDefaultRetryCount()
        {
            RetryCount++;
            _outputHelper.WriteLine($"[Retry Count]: {RetryCount}");
        }
        
        [When(@"I increment the retry count")]
        public void WhenIIncrementTheRetryCount()
        {
            RetryCount++;
            _outputHelper.WriteLine($"[Retry Count]: {RetryCount}");
        }
        
        [Then(@"the retry result should be (.*)")]
        public void ThenTheRetryResultShouldBe(int expected)
        {
            Assert.AreEqual(expected, RetryCount);
        }

        [Then(@"the retry result should be equal to config")]
        public void ThenTheRetryResultShouldBeEqualToConfig()
        {
            Assert.AreEqual(_configuration.MaxRetries, RetryCount);
        }

        [Then(@"the retry result should be equal to 1 or to config value")]
        public void ThenTheRetryResultShouldBeEqualToOneOr()
        {
            if (_configuration.ApplyGlobally)
                Assert.AreEqual(_configuration.MaxRetries, RetryCount);
            else
                Assert.AreEqual(1, RetryCount);
        }

        [When(@"I increment the no retry count")]
        public void WhenIIncrementTheNoRetryCount()
        {
            
        }

    }
}
