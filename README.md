# Firefox Saved Password Backup / Decrypt (firepass48)

**IMPORTANT LEGAL NOTICE:** This tool is for testing and educational purposes only. The author takes no responsibility for its use. Use at your own risk.

## Overview
Note: Does not decrypt the master key ( if used ) that is AES-256-CBC encrypted. This tool is designed to find and decrypt passwords saved in Firefox browsers. It utilizes the Network Security Services (NSS) library, specifically nss3.dll, to initialize cryptographic functions and decrypt data. It targets the included .NET version bundled with Windows 11 (.NET Framework 4.8).

## Technical Requirements
* 64-bit Windows operating system
* 64-bit Firefox installation (the tool is compiled for x64 and requires 64-bit nss3.dll)
* .NET Framework 4.8 or higher

## Features
* **NSSHelper class:**
   * Initialize the NSS library
   * Decrypt data using NSS functions
* **GetFirefoxPasswords method:**
   * Reads the logins.json file from a Firefox profile
   * Initializes the NSS library
   * Deserializes the JSON data
   * Iterates through the logins, decrypting usernames and passwords
   * Stores the decrypted data in PasswordEntry objects
* **Main method:**
   * Locates Firefox profiles on the system
   * Calls GetFirefoxPasswords for each profile
   * Prints out the decrypted URLs, usernames, and passwords

## Usage
1. Ensure you have a 64-bit Firefox installation
2. Place the executable in the main Firefox folder
3. Run the tool using the command prompt
4. The decrypted passwords will be displayed in the command window

## Troubleshooting
* If you get a "BadImageFormatException", verify that:
  * You're using 64-bit Firefox
  * The nss3.dll being accessed is the 64-bit version
* If initialization fails, ensure the executable has access to Firefox's profile directory

## Legal Notice
By using this tool, you agree that:
* You will only use it on systems and data for which you have explicit permission
* You understand the potential legal and ethical implications of password decryption
* You will not use this tool for any malicious or unauthorized purposes
* The author is not responsible for any consequences resulting from the use or misuse of this tool
