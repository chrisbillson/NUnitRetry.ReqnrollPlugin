using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;
using System.IO;

namespace NUnitRetry;

public sealed class NRetryCommand : DelegatingTestCommand
{
    private readonly int _retryCount;
    private readonly TextWriter _progress;

    public NRetryCommand(TestCommand innerCommand, int retryCount)
        : this(innerCommand, retryCount, TestContext.Progress) { }

    internal NRetryCommand(TestCommand innerCommand, int retryCount, TextWriter progress) : base(innerCommand)
    {
        _progress = progress ?? throw new ArgumentNullException(nameof(progress));

        if (retryCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        }

        _retryCount = retryCount;
    }

    public override TestResult Execute(TestExecutionContext context)
    {
        int count = _retryCount;
        var retryLog = new List<string>();

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

            if (count > 0)
            {
                var message = $"[NUnitRetry] '{context.CurrentTest.Name}' attempt {_retryCount - count}/{_retryCount} failed, retrying... Reason: {context.CurrentResult.Message}";
                _progress.WriteLine(message);
                retryLog.Add(message);

                // Clear result for retry, keeping the retry history in its output
                context.CurrentResult = context.CurrentTest.MakeTestResult();
                foreach (var line in retryLog)
                {
                    context.CurrentResult.OutWriter.WriteLine(line);
                }

                context.CurrentRepeatCount++; // increment Retry count for next iteration. will only happen if we are guaranteed another iteration
            }
        }

        return context.CurrentResult;
    }
}
