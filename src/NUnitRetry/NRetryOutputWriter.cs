using NUnit.Framework;

namespace NUnitRetry;

public sealed class NRetryOutputWriter : IRetryOutputWriter
{
    public static IRetryOutputWriter Instance { get; } = new NRetryOutputWriter();

    public void WriteLine(string message) => TestContext.Progress.WriteLine(message);

    private NRetryOutputWriter() { }
}
