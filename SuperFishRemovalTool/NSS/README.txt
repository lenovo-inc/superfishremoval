The items contained in this directory are part of Network Security Services by Mozilla.

These items are necessary to remove the SuperFish root certificate from the certificate 
store used by Mozilla applications including Firefox and Thunderbird.

While the entire NSS library is included in the source control, this utility only uses:
	freebl3.dll,
	libnspr4.dll,
	libplc4.dll,
	libplds4.dll,
	nss3.dll,
	nssckbi.dll,
	nssdbm3.dll,
	nssutil3.dll,
	smime3.dll,
	softokn3.dll,
	sqlite3.dll,
	ssl3.dll,
	certutil.exe

NSS 3.14.2 is tri-licensed under the MPL 1.1/GPL 2.0/LGPL 2.1

https://developer.mozilla.org/en-US/docs/Mozilla/Projects/NSS
