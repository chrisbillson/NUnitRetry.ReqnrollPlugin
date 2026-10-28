using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnitRetry;
using Shouldly;
using System;
using System.IO;

namespace RetryOnException.NUnit.Tests;

public class NRetryCommandTests
{
    [Test]
    public void ShouldCatchException()
    {
        (var context, var innerCommand, var testResult, var progress) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws<SystemException>();

        new NRetryCommand(innerCommand.Object, 1, progress)
            .Execute(context)
            .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldCatchNUnitException()
    {
        (var context, var innerCommand, var testResult, var progress) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new SystemException()));

        new NRetryCommand(innerCommand.Object, 1, progress)
            .Execute(context)
            .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldCatchOtherExceptions()
    {
        (var context, var innerCommand, var testResult, var progress) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws<ApplicationException>();

        new NRetryCommand(innerCommand.Object, 1, progress)
             .Execute(context)
             .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldCatchOtherNUnitException()
    {
        (var context, var innerCommand, var testResult, var progress) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException()));

        new NRetryCommand(innerCommand.Object, 1, progress)
            .Execute(context)
            .ShouldBeSameAs(testResult);
    }

    [Test]
    public void ShouldLogRetryToProgress()
    {
        (var context, var innerCommand, var testResult, var progress) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException("boom")));

        new NRetryCommand(innerCommand.Object, 2, progress)
            .Execute(context)
            .ShouldBeSameAs(testResult);

        Lines(progress).ShouldBe(
        [
            "[NUnitRetry] 'some test' attempt 1/2 failed, retrying... Reason: System.ApplicationException : boom"
        ]);
    }

    [Test]
    public void ShouldLogEachRetryInOrderExceptFinalAttempt()
    {
        (var context, var innerCommand, var testResult, var progress) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException("boom")));

        new NRetryCommand(innerCommand.Object, 3, progress)
            .Execute(context)
            .ShouldBeSameAs(testResult);

        Lines(progress).ShouldBe(
        [
            "[NUnitRetry] 'some test' attempt 1/3 failed, retrying... Reason: System.ApplicationException : boom",
            "[NUnitRetry] 'some test' attempt 2/3 failed, retrying... Reason: System.ApplicationException : boom"
        ]);
    }

    [Test]
    public void ShouldStopLoggingWhenRetryPasses()
    {
        (var context, var innerCommand, _, var progress) = Arrange();
        var passedResult = new Mock<TestResult>(context.CurrentTest).Object;
        passedResult.SetResult(ResultState.Success);

        innerCommand.SetupSequence(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException("boom")))
            .Returns(passedResult);

        new NRetryCommand(innerCommand.Object, 3, progress)
            .Execute(context)
            .ShouldBeSameAs(passedResult);

        Lines(progress).ShouldBe(
        [
            "[NUnitRetry] 'some test' attempt 1/3 failed, retrying... Reason: System.ApplicationException : boom"
        ]);
    }

    [Test]
    public void ShouldLogRetryWhenInnerCommandReturnsFailedResult()
    {
        (var context, var innerCommand, _, var progress) = Arrange();
        var failedResult = new Mock<TestResult>(context.CurrentTest).Object;
        failedResult.SetResult(ResultState.Failure, "Expected: 3 But was: 1");

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Returns(failedResult);

        new NRetryCommand(innerCommand.Object, 2, progress)
            .Execute(context)
            .ShouldBeSameAs(failedResult);

        Lines(progress).ShouldBe(
        [
            "[NUnitRetry] 'some test' attempt 1/2 failed, retrying... Reason: Expected: 3 But was: 1"
        ]);
    }

    [Test]
    public void ShouldNotLogWhenOnlyAttemptFails()
    {
        (var context, var innerCommand, _, var progress) = Arrange();

        innerCommand.Setup(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws<ApplicationException>();

        new NRetryCommand(innerCommand.Object, 1, progress)
            .Execute(context);

        progress.ToString().ShouldBeEmpty();
    }

    [Test]
    public void ShouldKeepRetryHistoryInFinalResultOutput()
    {
        (var context, var innerCommand, _, var progress) = Arrange();
        Mock.Get(context.CurrentTest)
            .Setup(t => t.MakeTestResult())
            .Returns(() => new Mock<TestResult>(context.CurrentTest).Object);

        innerCommand.SetupSequence(c => c.Execute(It.IsAny<TestExecutionContext>()))
            .Throws(new NUnitException("a message", new ApplicationException("boom")))
            .Throws(new NUnitException("a message", new ApplicationException("boom")))
            .Returns(() =>
            {
                context.CurrentResult.SetResult(ResultState.Success);
                return context.CurrentResult;
            });

        var result = new NRetryCommand(innerCommand.Object, 3, progress)
            .Execute(context);

        result.ResultState.ShouldBe(ResultState.Success);
        Lines(progress).Length.ShouldBe(2);
        result.Output.ShouldBe(progress.ToString());
    }

    [Test]
    public void ShouldThrowWhenProgressIsNull()
    {
        (_, var innerCommand, _, _) = Arrange();

        Should.Throw<ArgumentNullException>(() => new NRetryCommand(innerCommand.Object, 1, null))
            .ParamName.ShouldBe("progress");
    }

    private static string[] Lines(StringWriter writer) =>
        writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

    private (TestExecutionContext context, Mock<TestCommand> innerCommand, TestResult testResult, StringWriter progress) Arrange()
    {
        var test = new Mock<Test>("some test");
        var innerCommand = new Mock<TestCommand>(test.Object);
        var testResult = new Mock<TestResult>(test.Object);

        test.Setup(t => t.MakeTestResult()).Returns(testResult.Object);

        return (new TestExecutionContext
        {
            CurrentTest = test.Object
        },
            innerCommand,
            testResult.Object,
            new StringWriter());
    }
}
