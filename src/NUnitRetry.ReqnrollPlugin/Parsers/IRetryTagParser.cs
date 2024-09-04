namespace NUnitRetry.ReqnrollPlugin.Parsers
{
    public interface IRetryTagParser
    {
        RetryTag Parse(string tag);
    }
}
