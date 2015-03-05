using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool
{
    internal static class BrowserDetector
    {
        public static List<string>  CommonBrowserProcesses
        {
            get
            {
                return new List<String>()
                {
                    "firefox",
                    "chrome",
                    "iexplore",
                    "opera",
                    "safari",
                    "Maxthon",
                    "liebao", // "Cheetah fast browser", liebao.cn
                };
            }
        }
        public static bool AreAnyWebBrowsersRunning()
        {
            bool isAnyRunning = false;
            foreach(var browserName in BrowserDetector.CommonBrowserProcesses)
            {
                var processlist = System.Diagnostics.Process.GetProcessesByName(browserName);
                if(processlist != null && processlist.Any())
                {
                    isAnyRunning = true;
                    break;
                }
            }
            return isAnyRunning;
        }

    }
}
