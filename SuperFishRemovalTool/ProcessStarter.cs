using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool
{
    internal static class ProcessStarter
    {

        public static int StartWithWindow(string filename, bool Wait = false)
        {
            int exitCode = -1;

            System.Diagnostics.Process run = System.Diagnostics.Process.Start(filename);

            if (Wait)
            {
                if (run.WaitForExit(30000))
                {
                    exitCode = run.ExitCode;
                }
                else
                {
                    exitCode = -2;
                }
            }
            else
            {
                exitCode = 0;
            }

            return exitCode;
        }
        public static int StartWithoutWindow(string filename, string arguments, bool Wait = false)
        {
            int exitCode = -1;

            System.Diagnostics.Process run = new System.Diagnostics.Process();
            run.EnableRaisingEvents = true;

            run.StartInfo = new System.Diagnostics.ProcessStartInfo();
            run.StartInfo.RedirectStandardOutput = true;
            run.StartInfo.RedirectStandardError = true;
            //run.StartInfo.UseShellExecute = true;
            //run.StartInfo.Verb = "runas";
            run.StartInfo.UseShellExecute = false;
            run.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            run.StartInfo.CreateNoWindow = true;

            run.StartInfo.FileName = filename;
            if (null != arguments)
            {
                run.StartInfo.Arguments = arguments;
            }

            if (run.Start())
            {
                if (Wait)
                {
                    if (run.WaitForExit(30000))
                    {
                        exitCode = run.ExitCode;
                    }
                    else
                    {
                        exitCode = -2;
                    }
                }
                else
                {
                    exitCode = 0;
                }
            }

            return exitCode;
        }
    }
}
