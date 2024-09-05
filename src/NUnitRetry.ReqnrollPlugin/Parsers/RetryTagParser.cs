using System;
using System.Text.RegularExpressions;

namespace NUnitRetry.ReqnrollPlugin.Parsers
{
    public class RetryTagParser : IRetryTagParser
    {
        // unescaped: ^retry(\(([0-9]+)(,([0-9]+))?\))?$
        private readonly Regex regex = new Regex("^retry(\\(([0-9]+)(,([0-9]+))?\\))?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public RetryTag Parse(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            int? maxRetries = null;

            Match match = regex.Match(tag);
            if (match.Success)
            {
                // Group 2 is max retries
                if (match.Groups[2].Success)
                {
                    maxRetries = int.Parse(match.Groups[2].Value);
                }
            }

            return new RetryTag(maxRetries);
        }
    }
}
