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
            if (0 < args.Length)
            {
                // Redirect console output to parent process - Must be before any calls to Console.WriteLine ()                
                if (!AttachConsole(-1))  // ATTACH_PARENT_PROCESS = -1
                {
                    AllocConsole();
                }

                if (0 == String.Compare("/silent", args[0], true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    ExitCode = SilentMode();
                }
                else if (0 == String.Compare("/exist", args[0], true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    ExitCode = SilentMode(true);
                }
                else
                {
                    Logger.Log(Logging.LogSeverity.Information, "Switches supported are /silent and /exist");
                    ExitCode = 0;
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainScreen());
            }

            return ExitCode;
        }

        static int SilentMode(bool DetectOnly = false)
        {
            int ExitCode = 0;

            if (DetectOnly)
            {
                Logger.Log(Logging.LogSeverity.Information, "Superfish Detection Check");
            }
            else
            {
                Logger.Log(Logging.LogSeverity.Information, "Superfish Removal Check");
            }

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

                        Utilities.FixResult removalResult = agent.RemoveItem(DetectOnly);
                        if (removalResult.DidFail)
                        {
                            logAgentError("Error, failed while detecting / removing");
                        }
                        else if (removalResult.DidExist)
                        {
                            if ((!DetectOnly) && removalResult.WasRemoved)
                            {
                                logAgentInfo("Found and removed");
                            }
                            else
                            {
                                if (DetectOnly)
                                {
                                    logAgentError("Found");
                                }
                                else
                                {
                                    logAgentError("Found BUT NOT removed");
                                }

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
