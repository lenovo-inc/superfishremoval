using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Utilities
{
    class MozillaCertificateUtility : ISuperfishDetector
    {
        public string UtilityName { get { return Localization.LocalizationManager.Get().DetectorNameMozilla; } }
        
        private bool FoundCertFirefox = false;
        private bool FoundCertThunderbird = false;

        public bool DoesExist()
        {
            string FirefoxProfilesDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla\\Firefox\\Profiles");
            string ThunderbirdProfilesDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Thunderbird\\Profiles");

            if (System.IO.Directory.Exists(FirefoxProfilesDir))
            {
                if (FoundCertFirefox = HandleMozilla("Mozilla\\Firefox\\Profiles", false))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Firefox found - Certificate exists");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Firefox found - NO certificate exists");
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "  Firefox not found");
            }

            if (System.IO.Directory.Exists(ThunderbirdProfilesDir))
            {
                if (FoundCertThunderbird = HandleMozilla("Thunderbird\\Profiles", false))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Thunderbird found - Certificate exists");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Thunderbird found - NO certificate exists");
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "  Thunderbird not found");
            }

            bool retValue = (FoundCertFirefox) ? true : FoundCertThunderbird;
            return retValue;
        }

        public bool Remove()
        {
            // Couple of "problems/ToDos" here...
            //   1 - This ASSUMES the "DoesExit" ran first - I think that is "ok"
            //   2 - This will extract the tools TWICE - "probably" ok, but wasteful

            bool retValue = false;
            bool RemovedCertFirefox = false;
            bool RemovedCertThunderbird = false;

            if (FoundCertFirefox)
            {
                if (RemovedCertFirefox = HandleMozilla("Mozilla\\Firefox\\Profiles", true))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Firefox found - Certificate REMOVED");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Firefox found - Certificate NOT removed");
                }

                retValue = RemovedCertFirefox;
            }

            if (FoundCertThunderbird)
            {
                if (RemovedCertThunderbird = HandleMozilla("Thunderbird\\Profiles", true))
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Thunderbird found - Certificate REMOVED");
                }
                else
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Thunderbird found - Certificate NOT removed");
                }

                retValue = (FoundCertFirefox && (!RemovedCertFirefox)) ? false : RemovedCertThunderbird;
            }

            return retValue;
        }


        private bool HandleMozilla(string MainDir, bool remove = false)
        {
            bool result = false;

            // Build a list of directories to check for ALL users on the local system
            List<string> FirefoxProfilesDirs = new List<string>();
            string allUsersDirectory = System.IO.Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).FullName;
            string[] userDirectories = System.IO.Directory.GetDirectories(allUsersDirectory);
            foreach (string userDirectory in userDirectories)
            {
                string userAppDataDirectory = System.IO.Path.Combine(allUsersDirectory, userDirectory, "AppData");
                if (System.IO.Directory.Exists(userAppDataDirectory))
                {
                    string[] userLocalRoamingDirectories = System.IO.Directory.GetDirectories(userAppDataDirectory);
                    foreach (string userLocalRoamingDirectory in userLocalRoamingDirectories)
                    {
                        string firefoxProfilesDir = System.IO.Path.Combine(userAppDataDirectory, userLocalRoamingDirectory, MainDir);
                        if (System.IO.Directory.Exists(firefoxProfilesDir))
                        {
                            FirefoxProfilesDirs.Add(firefoxProfilesDir);
                        }
                    }
                }
            }
            // Just to be safe - Add original directory as well
            string MainAppdataDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MainDir);
            if (System.IO.Directory.Exists(MainAppdataDir))
            {
                if (! FirefoxProfilesDirs.Contains(MainAppdataDir))
                {
                    FirefoxProfilesDirs.Add(MainAppdataDir);
                }
            }            


            if ((null != FirefoxProfilesDirs) && (0 < FirefoxProfilesDirs.Count))
            {
                // Double-check for MSVCR100.dll
                if (System.IO.File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "msvcr100.dll")) ||
                    System.IO.File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "msvcr100.dll")) ||
                    System.IO.File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "msvcr100.dll")))
                {
                    string logging = "  Mozilla profiles found, extracing tools...";

                    string TempExtractDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SuperfishRemoval");
                    try
                    {
                        ExtractNSS(TempExtractDir);
                    }
                    catch (Exception ex)
                    {
                        logging += "  Exception trying to extract Mozilla certutil - " + ex.ToString() + "  ";
                    }
                    finally
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Information, logging + "done");
                    }

                    string certutilProgram = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SuperfishRemoval", "certutil.exe");

                    foreach (string FirefoxProfilesDir in FirefoxProfilesDirs)
                    {
                        string[] FirefoxProfiles = System.IO.Directory.GetDirectories(FirefoxProfilesDir, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                        foreach (string ProfileDir in FirefoxProfiles)
                        {
                            if (0 == RunCertutil(certutilProgram, ProfileDir, "Superfish, Inc.", remove))
                            {
                                result = true;
                            }
                            else
                            {
                                if (0 == RunCertutil(certutilProgram, ProfileDir, "Superfish, Inc. - Superfish, Inc.", remove))
                                {
                                    result = true;
                                }
                            }
                        }
                    }

                    try
                    {
                        System.Threading.Thread.Sleep(500);
                        System.IO.Directory.Delete(TempExtractDir, true);
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Information, "  Exception removing Mozilla tools - " + ex.ToString());
                        // Just try again...
                        try
                        {
                            System.IO.Directory.Delete(TempExtractDir, true);
                        }
                        catch { }
                    }

                    Logging.Logger.Log(Logging.LogSeverity.Information, "  Mozilla complete");
                }
                else
                {
                    if (! remove)
                    {
                        // This is a weird hack - but we cannot run the NSS cerutil.exe tool - so try to look inside the certificate store instead
                        // This only works for DETECTION, not removal
                        foreach (string FirefoxProfilesDir in FirefoxProfilesDirs)
                        {
                            string[] FirefoxProfiles = System.IO.Directory.GetDirectories(FirefoxProfilesDir, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                            foreach (string ProfileDir in FirefoxProfiles)
                            {
                                string certificateFilePath = System.IO.Path.Combine(ProfileDir, "cert8.db");
                                if (System.IO.File.Exists(certificateFilePath))
                                {
                                    // Reads in the entire file as a string and checks for references
                                    using (System.IO.StreamReader sr = new System.IO.StreamReader(certificateFilePath))
                                    {
                                        string fileContents = sr.ReadToEnd();
                                        // If "0" occurences of "superfish" are found - then cert was never installed
                                        // Apparently, if "17" occurences are found - then it IS installed
                                        // However, if only "16" are found - then it WAS installed, but it's not anymore?!?
                                        // Remember - this is just a backup hack in case NSS certutil.exe is not able to run
                                        int superfishCount = System.Text.RegularExpressions.Regex.Matches(fileContents.ToLower(), "superfish").Count;
                                        if (17 == superfishCount)
                                        {
                                            result = true;
                                            Logging.Logger.Log(Logging.LogSeverity.Information, "  Mozilla - Unable to run certutil.exe - FOUND Superfish certificate in " + certificateFilePath);
                                        }
                                        else if (0 == superfishCount)
                                        {
                                            Logging.Logger.Log(Logging.LogSeverity.Information, "  Mozilla - Unable to run certutil.exe - Superfish certificate NOT found in " + certificateFilePath);
                                        }
                                        else
                                        {
                                            Logging.Logger.Log(Logging.LogSeverity.Information, "  Mozilla - Unable to run certutil.exe - Superfish certificate found but REMOVED (" + superfishCount.ToString() + ") in " + certificateFilePath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Error, "  Mozilla Error - Looks like MSVCR100.dll does not exist - cannot run certutil.exe to remove certificate");
                    }
                }
            }
            else
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "  Skip Mozilla - Does not look like Profiles directories exists");
            }

            return result;
        }


        private int RunCertutil(string certutilProgram, string ProfileDir, string certName, bool remove = false)
        {
            int certutilResult = -1;

            string certutilArgs = null;
            if (remove)
            {
                certutilArgs = "-D -n \"" + certName + "\" -d \"" + ProfileDir + "\"";
            }
            else
            {
                certutilArgs = "-L -n \"" + certName + "\" -d \"" + ProfileDir + "\"";
            }

            string logging = "  Mozilla - Running: " + certutilProgram + " " + certutilArgs;
            
            try
            {
                certutilResult = ProcessStarter.StartWithoutWindow(certutilProgram, certutilArgs, true);
            }
            catch (Exception ex)
            {
                logging += "  Exception - " + ex.ToString();
            }
            finally
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, logging + "  Result = " + certutilResult);
            }

            return certutilResult;
        }


        private void ExtractNSS(string TempExtractDir)
        {
            string[] NSSFileList = new string[13] { 
                "freebl3.dll",
                "libnspr4.dll",
                "libplc4.dll",
                "libplds4.dll",
                "nss3.dll",
                "nssckbi.dll",
                "nssdbm3.dll",
                "nssutil3.dll",
                "smime3.dll",
                "softokn3.dll",
                "sqlite3.dll",
                "ssl3.dll",
                "certutil.exe"
            };

            if (System.IO.Directory.Exists(TempExtractDir))
            {
                System.IO.Directory.Delete(TempExtractDir, true);
            }
            System.IO.Directory.CreateDirectory(TempExtractDir);

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            foreach (string nssfile in NSSFileList)
            {
                var input = assembly.GetManifestResourceStream("SuperFishRemovalTool.NSS." + nssfile);
                var output = System.IO.File.Open(System.IO.Path.Combine(TempExtractDir, nssfile), System.IO.FileMode.CreateNew);

                CopyStream(input, output);

                output.Dispose();
                input.Dispose();
                output = null;
                input = null;
            }
        }

        private void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[32768];

            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);

                if (read <= 0)
                {
                    return;
                }

                output.Write(buffer, 0, read);
            }
        }
    }
}