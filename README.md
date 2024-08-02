# Firefox Saved Password Backup / Decrypt / Crack Tool

**IMPORTANT LEGAL NOTICE: This tool is for testing and educational purposes only. The author takes no responsibility for its use. Use at your own risk.**

## Overview

This tool is designed to find and decrypt passwords saved in Firefox browsers. It utilizes the Network Security Services (NSS) library, specifically `nss3.dll`, to initialize cryptographic functions and decrypt data.

## Features

The `NSSHelper` class provides methods to:
- Initialize the NSS library
- Decrypt data using NSS functions

The `GetFirefoxPasswords` method:
- Reads the `logins.json` file from a Firefox profile
- Initializes the NSS library
- Deserializes the JSON data
- Iterates through the logins, decrypting usernames and passwords
- Stores the decrypted data in `PasswordEntry` objects

The `Main` method:
- Locates Firefox profiles on the system
- Calls `GetFirefoxPasswords` for each profile
- Prints out the decrypted URLs, usernames, and passwords

## Usage

1. Place the executable in the main Firefox folder.
2. Run the tool using the command prompt.
3. The decrypted passwords will be displayed in the command window.

## Disclaimer

This tool is intended for legitimate testing and educational purposes only. The author assumes no liability for any misuse or damage caused by this tool. Users are solely responsible for ensuring they have proper authorization before using this tool on any system or data they do not own.

## Legal Notice

By using this tool, you agree that:
1. You will only use it on systems and data for which you have explicit permission.
2. You understand the potential legal and ethical implications of password decryption.
3. You will not use this tool for any malicious or unauthorized purposes.
4. The author is not responsible for any consequences resulting from the use or misuse of this tool.

Use this tool responsibly and ethically, and always respect privacy and security laws and regulations.
