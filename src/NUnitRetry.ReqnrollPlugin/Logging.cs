using System;
using System.IO;

namespace NUnitRetry.ReqnrollPlugin
{
    // Don't mind this class; it's for development and debugging purposes only.
    // Debugging of SpecFlow plugins is kinda pain, thus file with logs comes in handy
    public static class Logging
    {
        private static bool logging = false;
        private static string path = @"C:\Dev\logs.txt";

        public static void WriteLine(string methodName, string message)
        {
            if (logging)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine($"[{methodName}]: {message}");
                }
            }
        }
    }
}
