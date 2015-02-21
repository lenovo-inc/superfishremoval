# SuperFish Removal Utility

SuperFish was pre-installed on a limited group of Lenovo branded notebooks  beginning  September, 2014.  Lenovo recommends removing SuperFish and the SuperFish certificates from all systems.   

This utility will completely analyze your system for this problem and remove the SuperFish application, associated registry entries, files and security certificates, if needed. 

## Download
  - [Download from Lenovo.com](http://download.lenovo.com/pccbbs/thinkvantage_en/metroapps/SuperFishRemovalTool/1.0.2.0/Lenovo.SuperFishRemovalTool.exe)


## Links
  - [Manual Removal Instructions](http://support.lenovo.com/us/en/product_security/superfish_uninstall)
  - [Lenovo Security Advisory](http://support.lenovo.com/us/en/product_security/superfish)
  - [Lenovo Statement](http://news.lenovo.com/article_display.cfm?article_id=1929)
  - [Lenovo License Agreement](http://support.lenovo.com/us/en/documents/ht100141)

#### Version
1.0.2.0

#### License
MPL 1.1

#### Technical Operations
  - Uninstall Superfish application ( %ProgramFiles(x86)%\Lenovo\VisualDiscovery\uninstall.exe)
  - Removal of Root certificate within Trusted Root Certification Authorities ("Superfish, Inc")
  - Removal of Root certificate within Firefox and Thunderbird certificate stores ("Superfish, Inc")
  - Removal of remaining registry items under HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VisualDiscovery
  - Removal of remaining .ini and .log files
   * %SystemRoot%\SysWOW64\VisualDiscovery.ini
   * %SystemRoot%\SysWOW64\VisualDiscoveryOff.ini
   * %SystemRoot%\System32\VisualDiscoveryOff.ini
   * %TEMP%\VisualDiscoveryr.log

This tool can be run from command line.  From an Admin CMD prompt and run the following:

    start /w Lenovo.SuperFishRemovalTool.exe /silent

This reports the same status as the GUI and returns a non-zero error level on failure.  

##### Network Security Services (NSS)
[NSS](https://developer.mozilla.org/docs/Mozilla/Projects/NSS) is an open source library provided by [Mozilla](https://www.mozilla.org) that is necessary to remove the Superfish certificate from the Firefox and Thunderbird certificate store instances.  The NSS toolset is included in this project.
