using System;
using System.Collections.Generic;

namespace SuperFishRemovalTool.Utilities
{
    class FilesDetector : ISuperfishDetector
    {
        public string UtilityName { get { return Localization.LocalizationManager.Get().DetectorNameFile; } }

        public bool DoesExist()
        {            
            string[] allfiles = FindAllSuperfishFiles();

            if ((null != allfiles) && (0 < allfiles.Length))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish files: ");
                foreach (string file in allfiles)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  " + file);
                }

                return true;
            }

            return false;
        }

        public bool Remove()
        {
            bool FilesRemoved = false;

            string[] allfiles = FindAllSuperfishFiles();
            if ((null != allfiles) && (0 < allfiles.Length))
            {
                bool ProblemDeletingFile = false;

                Logging.Logger.Log(Logging.LogSeverity.Information, "Deleting Superfish files: ");
                foreach (string file in allfiles)
                {
                    Logging.Logger.Log(Logging.LogSeverity.Information, "  " + file);
                    try
                    {
                        if (System.IO.Directory.Exists(file))
                        {
                            System.IO.Directory.Delete(file, true);
                            if (System.IO.Directory.Exists(file))
                            {
                                ProblemDeletingFile = true;
                                Logging.Logger.Log(Logging.LogSeverity.Error, "  Error deleting " + file);
                            }
                        }
                        else
                        {
                            System.IO.File.Delete(file);
                            if (System.IO.File.Exists(file))
                            {
                                ProblemDeletingFile = true;
                                Logging.Logger.Log(Logging.LogSeverity.Error, "  Error deleting " + file);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ProblemDeletingFile = true;
                        Logging.Logger.Log(Logging.LogSeverity.Error, "  Exception deleting " + file + " - " + ex.ToString());
                    }
                }
                
                FilesRemoved = (!ProblemDeletingFile);
            }

            return FilesRemoved;
        }

        private string[] FindAllFiles(string folder, string pattern)
        {
            List<string> filelist = new List<string>();

            try
            {
                filelist.AddRange(System.IO.Directory.GetFiles(folder, pattern, System.IO.SearchOption.TopDirectoryOnly));
            }
            catch (Exception ex)
            {
                // Don't care if I can't read the directory
                if ((!(ex is UnauthorizedAccessException)) && (!(ex is System.IO.DirectoryNotFoundException)) && (!(ex is System.IO.PathTooLongException)))
                {
                    throw;
                }
            }

            try
            {
                foreach (string subDir in System.IO.Directory.GetDirectories(folder))
                {
                    try
                    {
                        filelist.AddRange(FindAllFiles(subDir, pattern));
                    }
                    catch (Exception ex)
                    {
                        // Don't care if I can't read the directory
                        if ((!(ex is UnauthorizedAccessException)) && (!(ex is System.IO.DirectoryNotFoundException)) && (!(ex is System.IO.PathTooLongException)))
                        {
                            throw;
                        }                    
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't care if I can't read the directory
                if ((!(ex is UnauthorizedAccessException)) && (!(ex is System.IO.DirectoryNotFoundException)) && (!(ex is System.IO.PathTooLongException)))
                {
                    throw;
                }
            }

            return filelist.ToArray();
        }

        private string[] FindAllSuperfishFiles()
        {
            List<string> AllFiles = new List<string>();

            // Check %ProgramFile% directories first
            string SuperfishProgramFilesDirX86 = System.IO.Path.Combine(GetProgramFilesX86(), "Lenovo\\VisualDiscovery");
            if (System.IO.Directory.Exists(SuperfishProgramFilesDirX86))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish directory: " + SuperfishProgramFilesDirX86);
                AllFiles.Add(SuperfishProgramFilesDirX86);
            }

            string SuperfishProgramFilesDir = System.IO.Path.Combine(GetProgramFiles(), "Lenovo\\VisualDiscovery");
            if (System.IO.Directory.Exists(SuperfishProgramFilesDir))
            {
                Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish directory: " + SuperfishProgramFilesDir);
                AllFiles.Add(SuperfishProgramFilesDir);
            }

            string[] SystemFiles = null;
            string[] SYSWOWFiles = null;
            try
            {
                SystemFiles = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"),
                    "VisualDiscovery*.ini",
                    System.IO.SearchOption.AllDirectories);

                SYSWOWFiles = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64"),
                    "VisualDiscovery*.ini",
                    System.IO.SearchOption.AllDirectories);
            }
            catch
            {
                SystemFiles = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"), "VisualDiscovery*.ini");
                SYSWOWFiles = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64"), "VisualDiscovery*.ini");
            }

            string[] UserFiles = null;
            try
            {
                UserFiles = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData"),
                    "VisualDiscovery*.log",
                    System.IO.SearchOption.AllDirectories);
            }
            catch
            {
                UserFiles = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData"), "VisualDiscovery*.log");
            }

            string[] TempFiles = null;
            try
            {
                TempFiles = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"),
                    "VisualDiscovery*.log",
                    System.IO.SearchOption.AllDirectories);
            }
            catch
            {
                TempFiles = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"), "VisualDiscovery*.log");
            }

            if ((null != SystemFiles) && (0 < SystemFiles.Length))
            {
                AllFiles.AddRange(SystemFiles);
            }

            if ((null != SYSWOWFiles) && (0 < SYSWOWFiles.Length))
            {
                AllFiles.AddRange(SYSWOWFiles);
            }

            if ((null != UserFiles) && (0 < UserFiles.Length))
            {
                AllFiles.AddRange(UserFiles);
            }

            if ((null != TempFiles) && (0 < TempFiles.Length))
            {
                AllFiles.AddRange(TempFiles);
            }

            return AllFiles.ToArray();
        }

        private string GetProgramFilesX86()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            if (! System.IO.Directory.Exists(dir))
            {
                dir = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return dir;
        }
        private string GetProgramFiles()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (! System.IO.Directory.Exists(dir))
            {
                dir = Environment.GetEnvironmentVariable("ProgramFiles");
            }

            return dir;
        }
    }
}