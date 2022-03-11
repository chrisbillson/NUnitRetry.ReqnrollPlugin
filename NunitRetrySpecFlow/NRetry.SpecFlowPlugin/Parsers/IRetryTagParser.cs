namespace NRetry.SpecFlowPlugin.Parsers
{
    public interface IRetryTagParser
    {
        RetryTag Parse(string tag);
    }
}
