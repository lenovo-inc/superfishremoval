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

        /*
        private bool IsSuperfishCert(System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            string Issuer = cert.Issuer;
            string IssuerName = cert.IssuerName.Name;

            return ((Issuer.ToLower().Contains("superfish, inc")) || (IssuerName.ToLower().Contains("superfish, inc")));
        }
        */

        private bool HandleMozilla(string MainDir, bool remove = false)
        {
            bool result = false;

            string FirefoxProfilesDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MainDir);

            if (System.IO.Directory.Exists(FirefoxProfilesDir))
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

                string[] FirefoxProfiles = System.IO.Directory.GetDirectories(FirefoxProfilesDir, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                foreach (string ProfileDir in FirefoxProfiles)
                {
                    string certutilArgs = null;
                    if (remove)
                    {
                        certutilArgs = "-D -n \"Superfish, Inc.\" -d \"" + ProfileDir + "\"";
                    }
                    else
                    {
                        certutilArgs = "-L -n \"Superfish, Inc.\" -d \"" + ProfileDir + "\"";
                    }

                    logging = "  Mozilla - Running: " + certutilProgram + " " + certutilArgs;

                    int certutilResult = -1;
                    try
                    {
                        certutilResult = ProcessStarter.StartWithoutWindow(certutilProgram, certutilArgs, true);

                        // ToDo: Handle multiple profile directories
                        if (0 == certutilResult)
                        {
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        logging += "  Exception - " + ex.ToString();
                    }
                    finally
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Information, logging + "  Result = " + certutilResult);
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
                Logging.Logger.Log(Logging.LogSeverity.Information, "  Skip Mozilla - Does not look like Profiles directory exists");
            }

            return result;
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