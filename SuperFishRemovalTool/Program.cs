using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperFishRemovalTool.Utilities;

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
            Logging.Logger.IsLoggingEnabled = true;
            Logging.Logger.AddLogger(new Logging.ConsoleLogger());
            //Logging.Logger.AddLogger(new Logging.FileLogger());

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

            Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Removal Check");

            var appChecker = new Utilities.ApplicationUtility();
            Utilities.FixResult appResult = appChecker.RemoveItem();
            if (appResult.DidFail)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Application: ERROR");
            }
            else if (appResult.DidExist)
            {
                if (appResult.WasRemoved)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Application: Found & Removed");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Application: Found - ERROR Removing");
                    ExitCode = 1;  //ExitCode = (0 == ExitCode) ? 1 : 0;
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Application: Not found");
            }

            var certChecker = new Utilities.CertificateUtility();
            Utilities.FixResult certResult = certChecker.RemoveItem();
            if (certResult.DidFail)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Certificates: ERROR");
            }
            else if (certResult.DidExist)
            {
                if (certResult.WasRemoved)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Certificates: Found & Removed");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Certificates: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 2 : 0;
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Certificates: Not found");
            }

            var certChecker2 = new Utilities.MozillaCertificateUtility();
            Utilities.FixResult certResult2 = certChecker2.RemoveItem();
            if (certResult2.DidFail)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Mozilla Certificates: ERROR");
            }
            else if (certResult2.DidExist)
            {
                if (certResult2.WasRemoved)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Mozilla Certificates: Found & Removed");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Mozilla Certificates: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 3 : 0;
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Mozilla Certificates: Not found");
            }

            var regChecker = new Utilities.RegistryUtility();
            Utilities.FixResult regResult = regChecker.RemoveItem();
            if (regResult.DidFail)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Registry Entries: ERROR");
            }
            else if (regResult.DidExist)
            {
                if (regResult.WasRemoved)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Registry Entries: Found & Removed");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Registry Entries: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 4 : 0;
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Registry Entries: Not found");
            }

            var fileChecker = new Utilities.FilesDetector();
            Utilities.FixResult fileResult = fileChecker.RemoveItem();
            if (fileResult.DidFail)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Files: ERROR");
            }
            else if (fileResult.DidExist)
            {
                if (fileResult.WasRemoved)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Files: Found & Removed");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Files: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 5 : 0;
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Files: Not found");
            }
            Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish Removal Check Complete");

            return ExitCode;
        }
    }
}
