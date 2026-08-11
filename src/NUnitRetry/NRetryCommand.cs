using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;

namespace NUnitRetry;

public sealed class NRetryCommand : DelegatingTestCommand
{
    private readonly int _retryCount;

    public NRetryCommand(TestCommand innerCommand, int retryCount) : base(innerCommand)
    {
        if (retryCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        }

        _retryCount = retryCount;
    }

    public override TestResult Execute(TestExecutionContext context)
    {
        int count = _retryCount;

        while (count-- > 0)
        {
            try
            {
                context.CurrentResult = innerCommand.Execute(context);
            }
            // Commands are supposed to catch exceptions, but some don't.
            catch (Exception ex)
            {
                context.CurrentResult ??= context.CurrentTest.MakeTestResult();
                context.CurrentResult.RecordException(ex);
            }

            if (context.CurrentResult.ResultState != ResultState.Failure && context.CurrentResult.ResultState != ResultState.Error)
            {
                break;
            }

            // Clear result for retry
            if (count > 0)
            {
                TestContext.Progress.WriteLine(
                    $"[NUnitRetry] '{context.CurrentTest.Name}' failed, retrying ({_retryCount - count}/{_retryCount})...");

                context.CurrentResult = context.CurrentTest.MakeTestResult();
                context.CurrentRepeatCount++; // increment Retry count for next iteration. will only happen if we are guaranteed another iteration
            }
        }

        return context.CurrentResult;
    }
}