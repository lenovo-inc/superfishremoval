using System;
using System.Collections.Generic;

namespace SuperFishRemovalTool.Utilities
{
    class FilesDetector : ISuperfishDetector
    {
        public string UtilityName { get { return Localizer.Get().DetectorNameFile; } }

        public bool DoesExist()
        {
            string[] allfiles = FindAllSuperfishFiles();

            if ((null != allfiles) && (0 < allfiles.Length))
            {
                Console.WriteLine("Found Superfish files: ");
                foreach (string file in allfiles)
                {
                    Console.WriteLine("  " + file);
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

                Console.WriteLine("Deleting Superfish files: ");
                foreach (string file in allfiles)
                {
                    Console.WriteLine("  " + file);
                    try
                    {
                        System.IO.File.Delete(file);

                        if (System.IO.File.Exists(file))
                        {
                            ProblemDeletingFile = true;
                            Console.WriteLine("  Error deleting " + file);
                        }
                    }
                    catch (Exception ex)
                    {
                        ProblemDeletingFile = true;
                        Console.WriteLine("  Exception deleting " + file + " - " + ex.ToString());
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

            return AllFiles.ToArray();
        }

    }
}