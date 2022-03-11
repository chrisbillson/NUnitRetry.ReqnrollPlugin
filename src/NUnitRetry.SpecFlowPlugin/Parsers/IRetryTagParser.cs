namespace NUnitRetry.SpecFlowPlugin.Parsers
{
    public interface IRetryTagParser
    {
        RetryTag Parse(string tag);
    }
}
