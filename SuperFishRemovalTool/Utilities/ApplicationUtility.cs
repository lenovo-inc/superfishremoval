using System;
using System.Collections.Generic;

namespace SuperFishRemovalTool.Utilities
{
    internal class ApplicationUtility : ISuperfishDetector
    {
        public const string REGISTRY_KEY_UNINSTALL = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
        public const string REGISTRY_KEY_UNINSTALL_WOW = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
        public const string REGISTRY_KEY_DISPLAYNAME = "DisplayName";
        public const string REGISTRY_KEY_PUBLISHER = "Publisher";
        public const string REGISTRY_KEY_UNINSTALLSTRING = "Uninstallstring";

        public string UtilityName { get { return Localization.LocalizationManager.Get().DetectorNameApp; } }

        public bool DoesExist()
        {
            bool FoundProcess = false;
            bool FoundInAddRemoveRegistry = false;

            // Move %ProgramFiles% check into "FilesDetector"... only look for "app" here
            // Instead, just see if process is running
            FoundProcess = VisualDiscoveryProcessExists();

            // Check uninstall key in registry - Make sure the uninstall program EXISTS too
            // Note: There may be some cases where the uninstall program doesn't exist anymore
            string UninstallWow = CheckAddRemoveRegistry(REGISTRY_KEY_UNINSTALL_WOW);
            if (FoundInAddRemoveRegistry = ((!String.IsNullOrWhiteSpace(UninstallWow)))) // && System.IO.File.Exists(UninstallWow)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish uninstall entry: " + UninstallWow);
            }
            else
            {
                string Uninstall = CheckAddRemoveRegistry(REGISTRY_KEY_UNINSTALL);
                if (FoundInAddRemoveRegistry = ((!String.IsNullOrWhiteSpace(Uninstall)))) // && System.IO.File.Exists(Uninstall)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish uninstall entry: " + Uninstall);
                }
            }

            return (FoundProcess || FoundInAddRemoveRegistry);
        }

        public bool Remove()
        {
            bool KilledProcess = KillVisualDiscoveryProcess();

            // Uninstall app if it exists in registry Add/Remove programs
            // FYI: Remove %ProgramFile% directory in FilesDetector if needed
            bool AppRemovedWow = UninstallSuperfish(REGISTRY_KEY_UNINSTALL_WOW);
            bool AppRemoved    = UninstallSuperfish(REGISTRY_KEY_UNINSTALL);

            return (KilledProcess || AppRemovedWow || AppRemoved);
        }


        private bool UninstallSuperfish(string AddRemoveRegistryKey)
        {
            bool FoundInAddRemoveRegistry = false;
            bool AppRemoved = false;

            string Uninstall = CheckAddRemoveRegistry(AddRemoveRegistryKey);
            if (FoundInAddRemoveRegistry = ((!String.IsNullOrWhiteSpace(Uninstall))))
            {
                // Try to stop/kill Superfish services and processes
                StopAllSuperfishProcesses();

                if (System.IO.File.Exists(Uninstall))
                {
                    AppRemoved = (0 == ProcessStarter.StartWithWindow(Uninstall, true));

                    // Apparently the uninstall does NOT wait - it returns right away
                    // So wait for the ProgramFiles directory to get removed
                    string SuperfishDir = System.IO.Directory.GetParent(Uninstall).FullName;
                    for (int i = 0; ((System.IO.Directory.Exists(SuperfishDir)) && (i < 60)); i++)
                    {
                        System.Threading.Thread.Sleep(500);
                    }

                    if (AppRemoved && (!System.IO.Directory.Exists(SuperfishDir)))
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish application removed");
                    }
                }
                else
                {
                    // Since uninstall does not do it - Try to remove Superfish services
                    RemoveAllSuperfishServices();

                    // Note: There may be some cases where the uninstall program doesn't exist anymore
                    //       In this case - Make sure Add/Remove registry key is DELETED
                    AppRemoved = (! String.IsNullOrWhiteSpace(CheckAddRemoveRegistry(AddRemoveRegistryKey, true)));
                }
            }

            return (FoundInAddRemoveRegistry && AppRemoved);
        }

        private string CheckAddRemoveRegistry(string uninstall_key, bool deletekey = false)
        {
            string AddRemoveUninstallString = null;

            Microsoft.Win32.RegistryKey mainkey = null;
            try
            {
                if (null != (mainkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstall_key, deletekey)))
                {
                    foreach (string keys in mainkey.GetSubKeyNames())
                    {
                        bool deletesubkey = false;
                        Microsoft.Win32.RegistryKey subkey = null;
                        try
                        {
                            if (null != (subkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstall_key + "\\" + keys)))
                            {
                                string name = subkey.GetValue(REGISTRY_KEY_DISPLAYNAME) as string;
                                string publisher = subkey.GetValue(REGISTRY_KEY_PUBLISHER) as string;
                                if ((IsSuperfishAppName(name)) || (IsSuperfishAppName(publisher)))
                                {
                                    string uninstall = subkey.GetValue(REGISTRY_KEY_UNINSTALLSTRING) as string;

                                    if (!String.IsNullOrWhiteSpace(uninstall))
                                    {
                                        AddRemoveUninstallString = uninstall;
                                        deletesubkey = deletekey;
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            if (null != subkey)
                            {
                                subkey.Close();
                            }

                            try
                            {
                                if (deletekey && deletesubkey)
                                {
                                    Logging.Logger.Log(Logging.LogSeverity.Information, "Removing Superfish application registry key - " + uninstall_key + "\\" + keys);
                                    mainkey.DeleteSubKeyTree(keys, false);
                                }
                            }
                            catch (Exception ex)
                            {
                                AddRemoveUninstallString = null;
                                Logging.Logger.Log(ex, "Exception trying to delete registry key - " + ex.ToString());
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (null != mainkey) mainkey.Close();
            }

            return AddRemoveUninstallString;
        }


        private bool IsSuperfishAppName(string displayname)
        {
            if (!String.IsNullOrWhiteSpace(displayname))
            {
                return ((displayname.ToLowerInvariant().Contains("superfish")));
            }

            return false;
        }

        public static void RemoveAllSuperfishServices()
        {
            if (0 == ProcessStarter.StartWithoutWindow("sc", "delete VisualDiscovery", true))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish service deleted");
            }
            // Not perfect - But give the service time to stop - just in case
            System.Threading.Thread.Sleep(500);

            if (0 == ProcessStarter.StartWithoutWindow("sc", "delete VDWFP", true))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "VisualDiscovery service deleted");
            }
            // Not perfect - But give the service time to stop - just in case
            System.Threading.Thread.Sleep(500);
        }

        public static bool StopAllSuperfishProcesses()
        {
            bool KilledProcess = false;

            if (0 == ProcessStarter.StartWithoutWindow("sc", "stop VisualDiscovery", true))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish service stopped");
            }
            // Not perfect - But give the service time to stop - just in case
            System.Threading.Thread.Sleep(500);

            if (0 == ProcessStarter.StartWithoutWindow("sc", "stop VDWFP", true))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "VisualDiscovery service stopped");
            }
            // Not perfect - But give the service time to stop - just in case
            System.Threading.Thread.Sleep(500);

            if (KilledProcess = KillVisualDiscoveryProcess())
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish processed stopped");
            }
            // Not perfect - But give the process time to stop - just in case
            System.Threading.Thread.Sleep(500);

            return KilledProcess;
        }

        private static bool KillVisualDiscoveryProcess()
        {
            bool ProcessKilled = false;

            try
            {
                System.Diagnostics.Process[] processlist = System.Diagnostics.Process.GetProcessesByName("VisualDiscovery");

                foreach (System.Diagnostics.Process p in processlist)
                {
                    p.Kill();
                    ProcessKilled = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception trying to kill process - " + ex.ToString());
            }

            return ProcessKilled;
        }

        private bool VisualDiscoveryProcessExists()
        {
            try
            {
                System.Diagnostics.Process[] processlist = System.Diagnostics.Process.GetProcessesByName("VisualDiscovery");
                return ((null != processlist) && (0 < processlist.Length));
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(ex, "Exception trying to detect process - " + ex.ToString());
            }

            return false;
        }
    }
}