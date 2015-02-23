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

            // Run in silent, console mode if requested
            if ((0 < args.Length) && (0 == String.Compare("/silent", args[0], true, System.Globalization.CultureInfo.InvariantCulture)))
            {
                // Redirect console output to parent process - Must be before any calls to Console.WriteLine()                
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

            Console.WriteLine("");
            Console.WriteLine("Superfish Removal Check");
            Console.WriteLine("");
            Console.WriteLine("");


            var appChecker = new Utilities.ApplicationUtility();
            Utilities.FixResult appResult = appChecker.RemoveItem();
            if (appResult.DidFail)
            {
                Console.WriteLine("Superfish Application: ERROR");
            }
            else if (appResult.DidExist)
            {
                if (appResult.WasRemoved)
                {
                    Console.WriteLine("Superfish Application: Found & Removed");
                }
                else
                {
                    Console.WriteLine("Superfish Application: Found - ERROR Removing");
                    ExitCode = 1;  //ExitCode = (0 == ExitCode) ? 1 : 0;
                }
            }
            else
            {
                Console.WriteLine("Superfish Application: Not found");
            }
            Console.WriteLine("");

            var certChecker = new Utilities.CertificateUtility();
            Utilities.FixResult certResult = certChecker.RemoveItem();
            if (certResult.DidFail)
            {
                Console.WriteLine("Superfish Certificates: ERROR");
            }
            else if (certResult.DidExist)
            {
                if (certResult.WasRemoved)
                {
                    Console.WriteLine("Superfish Certificates: Found & Removed");
                }
                else
                {
                    Console.WriteLine("Superfish Certificates: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 2 : 0;
                }
            }
            else
            {
                Console.WriteLine("Superfish Certificates: Not found");
            }
            Console.WriteLine("");

            var certChecker2 = new Utilities.MozillaCertificateUtility();
            Utilities.FixResult certResult2 = certChecker2.RemoveItem();
            if (certResult2.DidFail)
            {
                Console.WriteLine("Superfish Mozilla Certificates: ERROR");
            }
            else if (certResult2.DidExist)
            {
                if (certResult2.WasRemoved)
                {
                    Console.WriteLine("Superfish Mozilla Certificates: Found & Removed");
                }
                else
                {
                    Console.WriteLine("Superfish Mozilla Certificates: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 3 : 0;
                }
            }
            else
            {
                Console.WriteLine("Superfish Mozilla Certificates: Not found");
            }
            Console.WriteLine("");

            var regChecker = new Utilities.RegistryUtility();
            Utilities.FixResult regResult = regChecker.RemoveItem();
            if (regResult.DidFail)
            {
                Console.WriteLine("Superfish Registry Entries: ERROR");
            }
            else if (regResult.DidExist)
            {
                if (regResult.WasRemoved)
                {
                    Console.WriteLine("Superfish Registry Entries: Found & Removed");
                }
                else
                {
                    Console.WriteLine("Superfish Registry Entries: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 4 : 0;
                }
            }
            else
            {
                Console.WriteLine("Superfish Registry Entries: Not found");
            }
            Console.WriteLine("");

            var fileChecker = new Utilities.FilesDetector();
            Utilities.FixResult fileResult = fileChecker.RemoveItem();
            if (fileResult.DidFail)
            {
                Console.WriteLine("Superfish Files: ERROR");
            }
            else if (fileResult.DidExist)
            {
                if (fileResult.WasRemoved)
                {
                    Console.WriteLine("Superfish Files: Found & Removed");
                }
                else
                {
                    Console.WriteLine("Superfish Files: Found - ERROR Removing");
                    ExitCode = (0 == ExitCode) ? 5 : 0;
                }
            }
            else
            {
                Console.WriteLine("Superfish Files: Not found");
            }
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Superfish Removal Check Complete");
            Console.WriteLine("");

            return ExitCode;
        }
    }
}
