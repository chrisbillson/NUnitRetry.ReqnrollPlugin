using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnitRetry;
using Shouldly;
using System;

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
            .Throws(new NUnitException("a message", new ApplicationException()));

        new NRetryCommand(innerCommand.Object, 2, outputWriter.Object)
            .Execute(context)
            .ShouldBeSameAs(testResult);

        outputWriter.Verify(w => w.WriteLine("[NUnitRetry] 'some test' failed, retrying (1/2)..."), Times.Once);
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
