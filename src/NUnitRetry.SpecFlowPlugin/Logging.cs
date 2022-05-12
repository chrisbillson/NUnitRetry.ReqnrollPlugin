using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitRetry.SpecFlowPlugin
{
    // Don't mind this class; it's for development and debugging purposes only.
    // Debugging of SpecFlow plugins is kinda pain, thus file with logs comes in handy
    public static class Logging
    {
        private static Boolean logging = false;
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
