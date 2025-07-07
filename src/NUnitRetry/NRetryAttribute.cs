using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using RetryOnException.NUnit;
using System;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class NRetryAttribute : NUnitAttribute, IRepeatTest
    {
        private readonly int _retryCount;

        public NRetryAttribute(int retryCount)
        {
                
            if (retryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryCount));
            }
            _retryCount = retryCount;
        }

        public TestCommand Wrap(TestCommand command) =>
            new NRetryCommand(command, _retryCount);
    }
}