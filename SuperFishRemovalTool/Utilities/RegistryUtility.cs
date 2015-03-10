using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Utilities
{
    class RegistryUtility : ISuperfishDetector
    {
        public const string REGISTRY_KEY_UNINSTALL = "SOFTWARE\\VisualDiscovery";
        public const string REGISTRY_KEY_UNINSTALL32 = "SOFTWARE\\Wow6432Node\\VisualDiscovery";

        public string UtilityName { get { return Localization.LocalizationManager.Get().DetectorNameReg; } }

        public bool DoesExist()
        {
            bool RegistryKey32Found = CheckSoftwareRegistry(REGISTRY_KEY_UNINSTALL32);
            bool RegistryKey64Found = CheckSoftwareRegistry(REGISTRY_KEY_UNINSTALL);

            if (RegistryKey32Found)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish registry key found: HKLM\\SOFTWARE\\Wow6432Node\\VisualDiscovery");
            }

            if (RegistryKey64Found)
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish registry key found: HKLM\\SOFTWARE\\VisualDiscovery");
            }

            return (RegistryKey32Found || RegistryKey64Found);
        }

        public bool Remove()
        {
            bool RegistryKey32Removed = false;
            bool RegistryKey64Removed = false;

            if (CheckSoftwareRegistry(REGISTRY_KEY_UNINSTALL32))
            {
                if (RegistryKey32Removed = DeleteSoftwareRegistry("SOFTWARE\\Wow6432Node"))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish registry key removed: HKLM\\SOFTWARE\\Wow6432Node\\VisualDiscovery");
                }
            }

            if (CheckSoftwareRegistry(REGISTRY_KEY_UNINSTALL))
            {
                if (RegistryKey64Removed = DeleteSoftwareRegistry("SOFTWARE"))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish registry key removed: HKLM\\SOFTWARE\\VisualDiscovery");
                }
            }

            // Just a double-check - remove the add/remove key from the registry too
            if (CheckSoftwareRegistry(ApplicationUtility.REGISTRY_KEY_UNINSTALL_WOW + "\\" + ApplicationUtility.REGISTRY_KEY_SUPERFISH_UNINSTALL))
            {
                if (DeleteSoftwareRegistry(ApplicationUtility.REGISTRY_KEY_UNINSTALL_WOW, ApplicationUtility.REGISTRY_KEY_SUPERFISH_UNINSTALL))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish registry key removed: " + ApplicationUtility.REGISTRY_KEY_UNINSTALL_WOW + "\\" + ApplicationUtility.REGISTRY_KEY_SUPERFISH_UNINSTALL);
                }
            }
            if (CheckSoftwareRegistry(ApplicationUtility.REGISTRY_KEY_UNINSTALL + "\\" + ApplicationUtility.REGISTRY_KEY_SUPERFISH_UNINSTALL))
            {
                if (DeleteSoftwareRegistry(ApplicationUtility.REGISTRY_KEY_UNINSTALL, ApplicationUtility.REGISTRY_KEY_SUPERFISH_UNINSTALL))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "Superfish registry key removed: " + ApplicationUtility.REGISTRY_KEY_UNINSTALL + "\\" + ApplicationUtility.REGISTRY_KEY_SUPERFISH_UNINSTALL);
                }
            }

            return RegistryKey32Removed || RegistryKey64Removed;
        }

        private bool DeleteSoftwareRegistry(string uninstall_key, string keyname = null)
        {
            bool SoftwareKeyDeleted = false;

            Microsoft.Win32.RegistryKey mainkey = null;
            try
            {
                if (null != (mainkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstall_key, true)))
                {
                    try
                    {
                        if (String.IsNullOrWhiteSpace(keyname))
                        {
                            mainkey.DeleteSubKey("VisualDiscovery");
                        }
                        else
                        {
                            mainkey.DeleteSubKey(keyname);
                        }

                        SoftwareKeyDeleted = true;
                    }
                    catch { }
                }
            }
            catch { }
            finally
            {
                if (null != mainkey)
                {
                    mainkey.Close();
                    mainkey = null;
                }
            }

            return SoftwareKeyDeleted;
        }

        private bool CheckSoftwareRegistry(string uninstall_key)
        {
            bool SoftwareKeyExists = false;

            Microsoft.Win32.RegistryKey mainkey = null;
            try
            {
                SoftwareKeyExists = (null != (mainkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstall_key)));
            }
            catch { }
            finally
            {
                if (null != mainkey)
                {
                    mainkey.Close();
                    mainkey = null;
                }
            }

            return SoftwareKeyExists;
        }

    }
}