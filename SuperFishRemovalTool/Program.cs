using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperFishRemovalTool.Utilities;
using SuperFishRemovalTool.Logging;

namespace SuperFishRemovalTool
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            int ExitCode = 0;
            Logger.IsLoggingEnabled = true;
            Logger.AddLogger(new Logging.ConsoleLogger());
            //Logger.AddLogger(new Logging.FileLogger());

            // Run in silent, console mode if requested
            if ((0 < args.Length) && (0 == String.Compare("/silent", args[0], true, System.Globalization.CultureInfo.InvariantCulture)))
            {
                // Redirect console output to parent process - Must be before any calls to Console.WriteLine ()                
                if (!AttachConsole(-1))  // ATTACH_PARENT_PROCESS = -1
                {
                    AllocConsole();
                }

                ExitCode = SilentMode();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainScreen());
            }

            return ExitCode;
        }

        static int SilentMode()
        {
            int ExitCode = 0;

            Logger.Log(Logging.LogSeverity.Information, "Superfish Removal Check");

            var agents = Utilities.RemovalAgentFactory.GetRemovalAgents().ToList();
            if (agents != null && agents.Any())
            {
                int agentNumber = 1;
                foreach (var agent in agents)
                {

                    Action<string> logAgentInfo = (status) => { 
                        Logger.Log(Logging.LogSeverity.Information, "{0}: {1}", status, agent.UtilityName); 
                    };
                    Action<string> logAgentError = (status) =>{ 
                        Logger.Log(Logging.LogSeverity.Error, "{0}: {1}", status, agent.UtilityName); 
                    };

                    try
                    {
                        logAgentInfo("Working...");

                        Utilities.FixResult removalResult = agent.RemoveItem();
                        if (removalResult.DidFail)
                        {
                            logAgentError("Error, failed while detecting / removing");
                        }
                        else if (removalResult.DidExist)
                        {
                            if (removalResult.WasRemoved)
                            {
                                logAgentInfo("Found and removed");
                            }
                            else
                            {
                                logAgentError("Found BUT NOT removed");
                                // Error code should be the first error that occurs. 
                                ExitCode = (0 == ExitCode) ? agentNumber : ExitCode;
                            }
                        }
                        else
                        {
                            logAgentInfo("Not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex, "Exception while working on agent # {0}", agentNumber);
                    }
                    agentNumber++;
                } // End foreach agent
            }// End all agents

            Logger.Log(Logging.LogSeverity.Information, "Superfish Removal Check Complete!");
            Logger.Log(Logging.LogSeverity.Information, "Return code: {0}", ExitCode);

            return ExitCode;
        }
    }
}
