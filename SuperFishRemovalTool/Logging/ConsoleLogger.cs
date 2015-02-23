using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Logging
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(LogSeverity severity, string message)
        {
            string fullText = String.Format("Severity: {0}, \t\t Message: {1}", severity, message);
            //Console.WriteLine(String.Format("Date: {0:d/M/yyyy HH:mm:ss}", DateTime.Now));
            Console.WriteLine(fullText);
            Console.WriteLine("============================================================");
        }

        public void Log(Exception ex, string message)
        {
            if(ex != null)
            {
                //Console.WriteLine(String.Format("Date: {0:d/M/yyyy HH:mm:ss}", DateTime.Now));
                Console.WriteLine(String.Format("Exception: {0}, {1}", ex.GetType(), ex.Message));
                Console.WriteLine(String.Format("Message: {0}", message));
                Console.WriteLine("============================================================");
            }
        }
    }
}
