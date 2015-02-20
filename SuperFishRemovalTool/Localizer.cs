using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool
{
    static class Localizer
    {
        private static readonly Strings En = new Strings()
        {

            DetectorNameApp = @"SuperFish Application",
            DetectorNameReg = @"SuperFish Registry entries",
            DetectorNameCert = @"SuperFish Root Certificate",
            DetectorNameFile = @"SuperFish Files",
            DetectorNameMozilla = @"SuperFish Root Certificate for Mozilla products",

            Version = @"Version",
            UtilityAbout =
@"SuperFish was pre-installed on a limited group of Lenovo branded notebooks  beginning  September, 2014.  Lenovo recommends removing SuperFish and the SuperFish certificates from all systems.   

This utility will completely analyze your system for this problem and remove the SuperFish application, associated registry entries, files and security certificates, if needed.",
            
                                                                                                                                                                                       UtilityName = "SuperFish Removal Utility",
            SelectRemoveToStart = "Select 'Remove' to start",
            CloseWebBrowsers = @"Please close all browsers before continuing",
            Remove = "Analyze and Remove SuperFish Now",
            LearnMoreText = "Click here to learn about this tool",
            LearnMoreLocation = @"http://forums.lenovo.com/t5/Lenovo-P-Y-and-Z-series/Removal-Instructions-for-VisualDiscovery-Superfish-application/ta-p/2029206",
            RemovalCompleteMessage = @"",


            //Complete:  SuperFish was not found on this system and no action is required.
            OverallStatusAppRemoved = @"SuperFish was removed from this system.   We recommend restarting your system.",
            OverallStatusError = @"Error:  We could not remove one or more components.  We recommend you follow the manual removal instructions below.",
            OverallStatusNotOnSystem = @"Complete:  SuperFish was not found on this system and no action is required.",

            RestartNow = @"Restart now",
            RestartLater = @"Restart later",


            ResultFoundAndRemoved = @"Found and removed",
            ResultFoundButNotRemoved = @"Found but not removed",
            ResultNotFound = @"Not Found",
            Error = @"Error",

            MoreInformationText = "More information:",
            ManualRemovalInstructionsText = @"Manual Removal Instructions",
            LenovoSecurityAdvisoryText = @"Lenovo Security Advisory",
            LenovoStatementText = @"Lenovo Statement",
            LenovoStatementLink = @"http://news.lenovo.com/article_display.cfm?article_id=1929",
            ManualRemovalInstructionsLink = @"http://support.lenovo.com/us/en/product_security/superfish_uninstall",
            LenovoSecurityAdvisoryLink = @"http://support.lenovo.com/us/en/product_security/superfish",
            LicenseAgreementLink = @"http://support.lenovo.com/us/en/documents/ht100141",
            LicenseAgreementText = @"Lenovo License Agreement",
        };

        public static Strings Get()
        {
            return Localizer.Get("en");
        }

        public static Strings Get(string lang)
        {
            return En;
        }
    }

    public class Strings
    {
        public string UtilityAbout { get; set; }
        public string UtilityName { get; set; }
        public string Version { get; set; }

        public string DetectorNameApp { get; set; }
        public string DetectorNameReg { get; set; }
        public string DetectorNameCert { get; set; }
        public string DetectorNameFile { get; set; }
        public string DetectorNameMozilla { get; set; }

        public string SelectRemoveToStart { get; set; }

        public string StatusRegNotExists { get; set; }
        public string Remove { get; set; }
        public string CloseWebBrowsers { get; set; }
        public string LearnMoreText { get; set; }
        public string LearnMoreLocation { get; set; }
        public string RemovalCompleteMessage { get; set; }

        public string ResultFoundAndRemoved { get; set; }
        public string ResultFoundButNotRemoved { get; set; }
        public string ResultNotFound { get; set; }
        public string Error { get; set; }

        public string OverallStatusAppRemoved { get; set; }
        public string OverallStatusError { get; set; }
        public string OverallStatusNotOnSystem { get; set; }

        public string RestartNow { get; set; }
        public string RestartLater { get; set; }

        public string MoreInformationText { get; set; }
        public string ManualRemovalInstructionsText { get; set; }
        public string ManualRemovalInstructionsLink { get; set; }
        public string LenovoSecurityAdvisoryLink { get; set; }
        public string LenovoSecurityAdvisoryText { get; set; }
        public string LenovoStatementText { get; set; }
        public string LenovoStatementLink { get; set; }
        public string LicenseAgreementText { get; set; }
        public string LicenseAgreementLink { get; set; }
    }
}
