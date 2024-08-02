# firefox_saved_password_backup

- Find and decrypt passwords saved on Firefox

- It uses the Network Security Services (NSS) library, specifically nss3.dll, to initialize cryptographic functions and decrypt data.
The NSSHelper class provides methods to:

- Initialize the NSS library
Decrypt data using the NSS functions

- The GetFirefoxPasswords method:

 -Reads the logins.json file from a Firefox profile
 -Initializes the NSS library
 -Deserializes the JSON data
 -Iterates through the logins, decrypting usernames and passwords
 -Stores the decrypted data in PasswordEntry objects


- The Main method:

 -Locates Firefox profiles on the system
 -For each profile, it calls GetFirefoxPasswords
 -Prints out the decrypted URLs, usernames, and passwords

- USE:

- Place in the main Firefox folder, run with command prompt, the passwords will print in the CMD