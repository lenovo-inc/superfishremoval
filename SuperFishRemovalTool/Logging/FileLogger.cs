using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Logging
{
    internal class FileLogger : ILogger
    {
        private readonly string _pathToLogFile;
        public FileLogger()
        {
            _pathToLogFile = Path.GetTempFileName();
        }


        public void Log(LogSeverity severity, string message)
        {
            string text = String.Format("{0}\t\t{1}",severity, message);
            this.Log(text);
        }

        public void Log(Exception ex, string message)
        {
            string text = String.Format("{0}\t\t{1}\t{2}\r\n{3}",
                                      "Exception: ",
                                      ex.GetType(),
                                      message,
                                      ex.Message);
            this.Log(text);
        }

        private void Log(string text)
        {
            if (text != null)
            {
                try
                {
                    _writeSemaphore.WaitOne();
                    if (!File.Exists(_pathToLogFile))
                    {
                        using (StreamWriter txtWriter = File.CreateText(_pathToLogFile))
                        {
                            txtWriter.WriteLine(text);
                            txtWriter.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter txtWriter = File.AppendText(_pathToLogFile))
                        {
                            txtWriter.WriteLine(text);
                            txtWriter.Close();
                        }
                    }

                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(text + Environment.NewLine + ex.Message);
                }

                finally
                {
                    _writeSemaphore.Release();
                }
            }
        }

        private static System.Threading.Semaphore _writeSemaphore = new System.Threading.Semaphore(1, 1);
    }
}
