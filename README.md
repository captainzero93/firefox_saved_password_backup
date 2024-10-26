# Firefox Saved Password Backup / Decrypt (firepass48)

**IMPORTANT LEGAL NOTICE:** This tool is for testing and educational purposes only. The author takes no responsibility for its use. Use at your own risk.

## Overview

This tool is designed to find and decrypt passwords saved in Firefox browsers. It utilizes the Network Security Services (NSS) library, specifically nss3.dll, to initialize cryptographic functions and decrypt data. It targets the included .NET version bundled with Windows 11 (.NET Framework 4.8).

## Features

- **NSSHelper class:**
  - Initialize the NSS library
  - Decrypt data using NSS functions

- **GetFirefoxPasswords method:**
  - Reads the logins.json file from a Firefox profile
  - Initializes the NSS library
  - Deserializes the JSON data
  - Iterates through the logins, decrypting usernames and passwords
  - Stores the decrypted data in PasswordEntry objects

- **Main method:**
  - Locates Firefox profiles on the system
  - Calls GetFirefoxPasswords for each profile
  - Prints out the decrypted URLs, usernames, and passwords

## Usage

1. Place the executable in the main Firefox folder.
2. Run the tool using the command prompt.
3. The decrypted passwords will be displayed in the command window.

## Legal Notice

By using this tool, you agree that:
- You will only use it on systems and data for which you have explicit permission.
- You understand the potential legal and ethical implications of password decryption.
- You will not use this tool for any malicious or unauthorized purposes.
- The author is not responsible for any consequences resulting from the use or misuse of this tool.

Use this tool responsibly and ethically for research on private property only.

## Citation

If you use Firefox Saved Password Backup in your research or project, please cite it as follows:

[captainzero93]. (2024). Firefox Saved Password Backup. GitHub. https://github.com/captainzero93/firefox_saved_password_backup

By using this project, you agree to abide by the terms of the appropriate license based on your intended use.
