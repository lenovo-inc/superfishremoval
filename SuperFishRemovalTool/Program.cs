using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SuperFishRemovalTool.Utilities;
using SuperFishRemovalTool.Logging;
using System.Runtime.InteropServices;
using System.Linq;

namespace SuperFishRemovalTool
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            int ExitCode = 0;
            Logger.IsLoggingEnabled = true;
            Logger.AddLogger(new ConsoleLogger());
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

            Logger.Log(LogSeverity.Information, "Superfish Removal Check");

            //Tuple items: Detector, name to log, Error Code on failure.
            IEnumerable<Tuple<ISuperfishDetector, string, int>> detectors = getSuperfishDetectors();

            Action<string, string> logInformation = (utilityType, message) 
                => Logger.Log(LogSeverity.Information, String.Format("Superfish {0}: {1}", utilityType, message));

            foreach (var tuple in detectors)
            {
                Action<string> log = (message) => logInformation(tuple.Item2, message);
                ISuperfishDetector utility = tuple.Item1;
                FixResult result = utility.RemoveItem();
                if (result.DidFail)
                {
                    log("ERROR");
                }
                else if (result.DidExist)
                {
                    if (result.WasRemoved)
                    {
                        log("Found & Removed");
                    }
                    else
                    {
                        log("Found - ERROR Removing");
                        //We want the error code to be set to the first error that occurs. 
                        ExitCode = (ExitCode == 0) ? tuple.Item3 : ExitCode;
                    }
                }
                else
                {
                    log("Not found");
                }
            }
            
            return ExitCode;
        }


        private static IEnumerable<Tuple<ISuperfishDetector, string, int>> getSuperfishDetectors()
        {
            //Allow each individual constructor to throw an exception without affecting the creation status of the other detectors.
            Func<Func<ISuperfishDetector>, string, int, Tuple<ISuperfishDetector, string, int>> create = (detectorCreator, name, errorCode) =>
            {
                try { return new Tuple<ISuperfishDetector, string, int>(detectorCreator(), name, errorCode); }
                catch (Exception e) { Logger.Log(e, "Failed to create Superfish Detector"); return null; }
            };

            var ret = new List<Tuple<ISuperfishDetector, string, int>>
            {
                create(() => new ApplicationUtility(), "Application", 1),
                create(() => new CertificateUtility(), "Certificates", 2),
                create(() => new MozillaCertificateUtility(), "Mozilla Certificates", 3),
                create(() => new RegistryUtility(), "Registry Entries", 4),
                create(() => new FilesDetector(), "Files", 5),
            };

            //Remove any utility which threw an exception.
            return from utility in ret where utility != null select utility;
        }
    }
}
