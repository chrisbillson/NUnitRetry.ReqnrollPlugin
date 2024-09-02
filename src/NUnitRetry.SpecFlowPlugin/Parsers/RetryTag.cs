namespace NUnitRetry.SpecFlowPlugin.Parsers
{
    public class RetryTag
    {
        public readonly int? MaxRetries;

        public RetryTag(int? maxRetries)
        {
            MaxRetries = maxRetries;
        }
    }
}
