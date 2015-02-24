using System;
using System.Collections.Generic;

namespace SuperFishRemovalTool.Utilities
{
    internal class ApplicationUtility : ISuperfishDetector
    {
        public const string REGISTRY_KEY_UNINSTALL = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
        public const string REGISTRY_KEY_UNINSTALL32 = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
        public const string REGISTRY_KEY_DISPLAYNAME = "DisplayName";
        public const string REGISTRY_KEY_PUBLISHER = "Publisher";
        public const string REGISTRY_KEY_UNINSTALLSTRING = "Uninstallstring";
        //public const string REGISTRY_KEY_PARENTNAME = "ParentDisplayName";
        //public const string REGISTRY_KEY_SYSCOMPONENT = "SystemComponent";
        //public const string REGISTRY_KEY_WININSTALLER = "WindowsInstaller";
        //public const string REGISTRY_KEY_DISPLAYICO = "DisplayIcon";
        //public const string REGISTRY_KEY_DISPLAYVER = "DisplayVersion";

        public string UtilityName { get { return Localization.LocalizationManager.Get().DetectorNameApp; } }

        public bool DoesExist()
        {
            bool FoundInProgramFiles = false;
            bool FoundInAddRemoveRegistry = false;

            // Check %ProgramFile% directory first
            string SuperfishDir = GetSuperfishProgramFiles();
            if (FoundInProgramFiles = (!String.IsNullOrWhiteSpace(SuperfishDir)))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish directory: " + SuperfishDir);
            }


            // Check uninstall key in registry
            string Uninstall32 = CheckAddRemoveRegistry(REGISTRY_KEY_UNINSTALL32);
            string Uninstall64 = CheckAddRemoveRegistry(REGISTRY_KEY_UNINSTALL);
            if (FoundInAddRemoveRegistry = (
                (!String.IsNullOrWhiteSpace(Uninstall32)) ||
                (!String.IsNullOrWhiteSpace(Uninstall64))
                ))
            {
                if (!String.IsNullOrWhiteSpace(Uninstall32))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish uninstall entry: " + Uninstall32);
                }
                if (!String.IsNullOrWhiteSpace(Uninstall64))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish uninstall entry: " + Uninstall64);
                }
            }

            return FoundInProgramFiles || FoundInAddRemoveRegistry;
        }

        public bool Remove()
        {
            bool AppRemoved32 = false;
            bool AppRemoved64 = false;
            bool DirRemoved32 = false;
            bool DirRemoved64 = false;
            bool AppFound32 = false;
            bool AppFound64 = false;
            bool DirFound32 = false;
            bool DirFound64 = false;

            
            string Uninstall32 = CheckAddRemoveRegistry(REGISTRY_KEY_UNINSTALL32);
            if (!String.IsNullOrWhiteSpace(Uninstall32))
            {
                AppFound32 = true;

                if (0 == ProcessStarter.StartWithoutWindow("sc", "stop VisualDiscovery", true))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish service stopped");
                }

                if (KillVisualDiscoveryProcess())
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish processed stopped");
                }

                AppRemoved32 = (0 == ProcessStarter.StartWithWindow(Uninstall32, true));

                // Apparently the uninstall does NOT wait - it returns right away
                // So wait for the ProgramFiles directory to get removed
                string SuperfishDir = GetSuperfishProgramFiles();
                for (int i = 0; ((System.IO.Directory.Exists(SuperfishDir)) && (i < 60)); i++)
                {
                    System.Threading.Thread.Sleep(500);
                }

                if (AppRemoved32 && (!System.IO.Directory.Exists(SuperfishDir)))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish application removed");
                }
            }

            string Uninstall64 = CheckAddRemoveRegistry(REGISTRY_KEY_UNINSTALL);
            if (!String.IsNullOrWhiteSpace(Uninstall64))
            {
                AppFound64 = true;

                if (0 == ProcessStarter.StartWithoutWindow("sc", "stop VisualDiscovery", true))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish service stopped");
                }

                if (KillVisualDiscoveryProcess())
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish processed stopped");
                }

                AppRemoved64 = (0 == ProcessStarter.StartWithWindow(Uninstall64, true));

                // Apparently the uninstall does NOT wait - it returns right away
                // So wait for the ProgramFiles directory to get removed
                string SuperfishDir = GetSuperfishProgramFiles();
                for (int i = 0; ((System.IO.Directory.Exists(SuperfishDir)) && (i < 60)); i++)
                {
                    System.Threading.Thread.Sleep(500);
                }

                if (AppRemoved64 && (!System.IO.Directory.Exists(SuperfishDir)))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish application removed");
                }
            }

            // Check %ProgramFile% directory as well
            string SuperfishCheckDir = GetSuperfishProgramFiles();
            if (!String.IsNullOrWhiteSpace(SuperfishCheckDir))
            {
                DirFound32 = true;

                try
                {
                    System.IO.Directory.Delete(SuperfishCheckDir, true);
                }
                catch (Exception ex)
                {
                    Logging.Logger.Log(ex, ("Exception trying to remove directory: " + SuperfishCheckDir + " - " + ex.ToString()));
                }

                if (!(System.IO.Directory.Exists(SuperfishCheckDir)))
                {
                    DirRemoved32 = true;

                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish application directory removed: " + SuperfishCheckDir);
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish application directory NOT removed: " + SuperfishCheckDir);
                }                
            }

            SuperfishCheckDir = GetSuperfishProgramFiles();
            if (!String.IsNullOrWhiteSpace(SuperfishCheckDir))
            {
                DirFound64 = true;

                try
                {
                    System.IO.Directory.Delete(SuperfishCheckDir, true);
                }
                catch (Exception ex)
                {
                    Logging.Logger.Log(ex, "Exception trying to remove directory: " + SuperfishCheckDir + " - " + ex.ToString());
                }

                if (!(System.IO.Directory.Exists(SuperfishCheckDir)))
                {
                    DirRemoved64 = true;

                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish application directory removed: " + SuperfishCheckDir);
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish application directory NOT removed: " + SuperfishCheckDir);
                }
            }

            // This logic seems weird, but it should work
            int numFail = 0;

            if ((!AppFound32) && (!AppFound64) && (!DirFound32) && (!DirFound64))
            {
                // Nothing failed - but that is because nothing was FOUND
                // Since we did nothing, then we should return false below
                numFail = 1;
            }
            else
            {
                // We "found" something... so see if any of them failed
                if (AppFound32 && (!AppRemoved32)) { numFail++; }
                if (AppFound64 && (!AppRemoved64)) { numFail++; }
                if (DirFound32 && (!DirRemoved32)) { numFail++; }
                if (DirFound64 && (!DirRemoved64)) { numFail++; }
            }

            return (0 == numFail);
        }

        private string CheckAddRemoveRegistry(string uninstall_key)
        {
            string AddRemoveUninstallString = null;

            Microsoft.Win32.RegistryKey mainkey = null;
            try
            {
                if (null != (mainkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstall_key)))
                {
                    foreach (string keys in mainkey.GetSubKeyNames())
                    {
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
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            if (null != subkey) subkey.Close();
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

        private string GetSuperfishProgramFiles()
        {
            string SuperfishProgramFilesDir = null;

            // Check %ProgramFile% directory first
            foreach (string path in GetProgramFilesDirectories())
            {
                string tempdir = System.IO.Path.Combine(path, "Lenovo\\VisualDiscovery");
                if (System.IO.Directory.Exists(tempdir))
                {
                    SuperfishProgramFilesDir = tempdir;
                    break;
                }
            }

            return SuperfishProgramFilesDir;
        }

        private bool IsSuperfishAppName(string displayname)
        {
            if (!String.IsNullOrWhiteSpace(displayname))
            {
                return ((displayname.ToLowerInvariant().Contains("superfish")));
            }

            return false;
        }

        private List<string> GetProgramFilesDirectories()
        {
            List<string> ProgramFilesDirectories = new List<string>();

            ProgramFilesDirectories.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

            if (!ProgramFilesDirectories.Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)))
            {
                ProgramFilesDirectories.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            }

            if (!ProgramFilesDirectories.Contains(Environment.GetEnvironmentVariable("ProgramFiles(x86)")))
            {
                ProgramFilesDirectories.Add(Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
            }

            if (!ProgramFilesDirectories.Contains(Environment.GetEnvironmentVariable("ProgramFiles")))
            {
                ProgramFilesDirectories.Add(Environment.GetEnvironmentVariable("ProgramFiles"));
            }

            return ProgramFilesDirectories;
        }

        private bool KillVisualDiscoveryProcess()
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

        

        

    }
}