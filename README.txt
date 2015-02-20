Application Installation Location: 
	%ProgramFiles(x86)%\Lenovo\VisualDiscovery\

Windows Service Name:
	VisualDiscovery
	
Uninstall Application: The uninstaller runs silently, there is no UI.
  %ProgramFiles(x86)%\Lenovo\VisualDiscovery\uninstall.exe

==================== 
The following items are not removed by the installer.

  
Registry:
	HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VisualDiscovery

Certificate: (Run as admin when deleting)
	LocalComputer\Trusted Root Certification Authorities\Certificates
		"Superfish, Inc"

Files:
  %SystemRoot%\SysWOW64\VisualDiscovery.ini
  %SystemRoot%\SysWOW64\VisualDiscoveryOff.ini
  %SystemRoot%\System32\VisualDiscoveryOff.ini
  %TEMP%\VisualDiscoveryr.log

=====================

There are many binary files in the NSS directory.
The items contained in this directory are part of Network Security Services by Mozilla.

These items are necessary to remove the SuperFish root certificate from the certificate 
store used by Mozilla applications including Firefox and Thunderbird.

NSS 3.12.4 is tri-licensed under the MPL 1.1/GPL 2.0/LGPL 2.1

https://developer.mozilla.org/en-US/docs/Mozilla/Projects/NSS
