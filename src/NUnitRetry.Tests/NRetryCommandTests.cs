using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnitRetry;
using Shouldly;
using System;
using System.Collections.Generic;

namespace RetryOnException.NUnit.Tests;

public class NRetryCommandTests
{
    [Test]
    public void ShouldCatchException()
    {
        (var context, var innerCommand, var testResult, var outputWriter) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws<SystemException>();

        new NRetryCommand(innerCommand.Object, 1, outputWriter.Object)
            .Execute(context)
            .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldCatchNUnitException()
    {
        (var context, var innerCommand, var testResult, var outputWriter) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new SystemException()));

        new NRetryCommand(innerCommand.Object, 1, outputWriter.Object)
            .Execute(context)
            .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldCatchOtherExceptions()
    {
        (var context, var innerCommand, var testResult, var outputWriter) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws<ApplicationException>();

        new NRetryCommand(innerCommand.Object, 1, outputWriter.Object)
             .Execute(context)
             .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldCatchOtherNUnitException()
    {
        (var context, var innerCommand, var testResult, var outputWriter) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException()));

        new NRetryCommand(innerCommand.Object, 1, outputWriter.Object)
            .Execute(context)
            .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldLogRetryToOutputStream()
    {
        (var context, var innerCommand, var testResult, var outputWriter) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException("boom")));

        new NRetryCommand(innerCommand.Object, 2, outputWriter.Object)
            .Execute(context)
            .ShouldBeSameAs(testResult);

        outputWriter.Verify(w => w.WriteLine("[NUnitRetry] 'some test' attempt 1/2 failed, retrying... Reason: System.ApplicationException : boom"), Times.Once);
        outputWriter.VerifyNoOtherCalls();
    }

    [Test]
    public void ShouldLogEachRetryInOrderExceptFinalAttempt()
    {
        (var context, var innerCommand, var testResult, var outputWriter) = Arrange();
        var messages = new List<string>();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException("boom")));
        outputWriter.Setup(w => w.WriteLine(It.IsAny<string>())).Callback<string>(messages.Add);

        new NRetryCommand(innerCommand.Object, 3, outputWriter.Object)
            .Execute(context)
            .ShouldBeSameAs(testResult);

        messages.ShouldBe(
        [
            "[NUnitRetry] 'some test' attempt 1/3 failed, retrying... Reason: System.ApplicationException : boom",
            "[NUnitRetry] 'some test' attempt 2/3 failed, retrying... Reason: System.ApplicationException : boom"
        ]);
    }

    [Test]
    public void ShouldStopLoggingWhenRetryPasses()
    {
        (var context, var innerCommand, _, var outputWriter) = Arrange();
        var passedResult = new Mock<TestResult>(context.CurrentTest).Object;
        passedResult.SetResult(ResultState.Success);

        innerCommand.SetupSequence(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException("boom")))
            .Returns(passedResult);

        new NRetryCommand(innerCommand.Object, 3, outputWriter.Object)
            .Execute(context)
            .ShouldBeSameAs(passedResult);

        outputWriter.Verify(w => w.WriteLine("[NUnitRetry] 'some test' attempt 1/3 failed, retrying... Reason: System.ApplicationException : boom"), Times.Once);
        outputWriter.VerifyNoOtherCalls();
    }

    private (TestExecutionContext context, Mock<TestCommand> innerCommand, TestResult testResult, Mock<IRetryOutputWriter> outputWriter) Arrange()
    {
        var test = new Mock<Test>("some test");
        var innerCommand = new Mock<TestCommand>(test.Object);
        var testResult = new Mock<TestResult>(test.Object);
        var outputWriter = new Mock<IRetryOutputWriter>();

        test.Setup(t => t.MakeTestResult()).Returns(testResult.Object);

        return (new TestExecutionContext
        {
            CurrentTest = test.Object
        },
            innerCommand,
            testResult.Object,
            outputWriter);
    }
}
