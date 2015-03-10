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

                // Try to stop/kill AND remove Superfish services and processes (so that the files can be deleted)
                ApplicationUtility.StopAllSuperfishProcesses();
                ApplicationUtility.RemoveAllSuperfishServices();

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
                                System.IO.Directory.Delete(file, true);
                            }
                        }
                        else if (System.IO.File.Exists(file))
                        {
                            System.IO.File.Delete(file);
                            if (System.IO.File.Exists(file))
                            {
                                ProblemDeletingFile = true;
                                Logging.Logger.Log(Logging.LogSeverity.Error, "  Error deleting " + file);
                                System.IO.File.Delete(file);
                            }
                        }
                        else
                        {
                            // In case duplicate files were added to the list and removed earlier
                            Logging.Logger.Log(Logging.LogSeverity.Information, "  Already deleted " + file);
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
            try
            {
                string programfilesx86 = GetProgramFilesX86();
                if (null != programfilesx86)
                {
                    string SuperfishProgramFilesDirX86 = System.IO.Path.Combine(programfilesx86, "Lenovo\\VisualDiscovery");
                    if (System.IO.Directory.Exists(SuperfishProgramFilesDirX86))
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish directory: " + SuperfishProgramFilesDirX86);
                        AllFiles.Add(SuperfishProgramFilesDirX86);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(Logging.LogSeverity.Error, "Problem detecting ProgramFilesX86 - " + ex.ToString());
            }

            try
            {
                string programfiles = GetProgramFiles();
                if (null != programfiles)
                {
                    string SuperfishProgramFilesDir = System.IO.Path.Combine(programfiles, "Lenovo\\VisualDiscovery");
                    if (System.IO.Directory.Exists(SuperfishProgramFilesDir))
                    {
                        Logging.Logger.Log(Logging.LogSeverity.Information, "Found Superfish directory: " + SuperfishProgramFilesDir);
                        AllFiles.Add(SuperfishProgramFilesDir);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.Log(Logging.LogSeverity.Error, "Problem detecting ProgramFiles - " + ex.ToString());
            }

            string[] SystemFiles = null;
            string[] SYSWOWFiles = null;
            string[] SysDriver = null, SysDriver64 = null;
            string[] SYSWOWDriver = null, SYSWOWDriver64 = null;
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

                SysDriver = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"),
                    "VDWFP.sys",
                    System.IO.SearchOption.AllDirectories);

                SysDriver64 = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"),
                    "VDWFP64.sys",
                    System.IO.SearchOption.AllDirectories);

                SYSWOWDriver = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64"),
                    "VDWFP.sys",
                    System.IO.SearchOption.AllDirectories);

                SYSWOWDriver64 = System.IO.Directory.GetFiles(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64"),
                    "VDWFP64.sys",
                    System.IO.SearchOption.AllDirectories);
            }
            catch
            {
                SystemFiles = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"), "VisualDiscovery*.ini");
                SYSWOWFiles = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64"), "VisualDiscovery*.ini");

                SysDriver = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"), "VDWFP.sys");
                SysDriver64 = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32"), "VDWFP64.sys");
                SYSWOWDriver = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64"), "VDWFP.sys");
                SYSWOWDriver64 = FindAllFiles(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SysWOW64"), "VDWFP64.sys");
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

            if ((null != SysDriver) && (0 < SysDriver.Length))
            {
                AllFiles.AddRange(SysDriver);
            }
            if ((null != SysDriver64) && (0 < SysDriver64.Length))
            {
                AllFiles.AddRange(SysDriver64);
            }
            if ((null != SYSWOWDriver) && (0 < SYSWOWDriver.Length))
            {
                AllFiles.AddRange(SYSWOWDriver);
            }
            if ((null != SYSWOWDriver64) && (0 < SYSWOWDriver64.Length))
            {
                AllFiles.AddRange(SYSWOWDriver64);
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

            //if (! System.IO.Directory.Exists(dir))
            //{
            //    dir = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            //}

            if (String.IsNullOrWhiteSpace(dir))
            {
                dir = GetProgramFiles();
            }            

            return dir;
        }

        private string GetProgramFiles()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            //if (! System.IO.Directory.Exists(dir))
            //{
            //    dir = Environment.GetEnvironmentVariable("ProgramFiles");
            //}

            if (String.IsNullOrWhiteSpace(dir))
            {
                dir = null;
            }

            return dir;
        }
    }
}