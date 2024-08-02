using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json; // Use Newtonsoft.Json instead of System.Text.Json for net 48

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
        return NSS_Init(nssPath) == 0;
    }

    public static string DecryptData(string encryptedData)
    {
        var decodedData = Convert.FromBase64String(encryptedData);
        var tsecData = new TSECItem
        {
            data = Marshal.AllocHGlobal(decodedData.Length),
            len = decodedData.Length
        };
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

        return null;
    }

    public static List<PasswordEntry> GetFirefoxPasswords(string loginsPath, string nssPath)
    {
        var passwordList = new List<PasswordEntry>();

        if (!File.Exists(loginsPath))
        {
            Console.WriteLine("[ERR] Firefox logins.json not found.");
            return passwordList; // Return an empty list
        }

        if (!NSSHelper.Initialize(nssPath))
        {
            Console.WriteLine("[ERR] NSS initialization failed.");
            return passwordList; // Return an empty list
        }

        var loginsJson = File.ReadAllText(loginsPath);
        dynamic logins = JsonConvert.DeserializeObject(loginsJson); // Deserialize with Newtonsoft.Json

        foreach (var login in logins.logins)
        {
            var url = login.hostname;
            var usernameEncrypted = login.encryptedUsername;
            var passwordEncrypted = login.encryptedPassword;

            var username = DecryptData(usernameEncrypted);
            var password = DecryptData(passwordEncrypted);

            passwordList.Add(new PasswordEntry
            {
                Url = url,
                Username = username,
                Password = password
            });
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
        var profilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox", "Profiles");
        var profiles = Directory.GetDirectories(profilesPath);

        foreach (var profile in profiles)
        {
            Console.WriteLine($"Processing Firefox profile: {profile}...");

            var loginsPath = Path.Combine(profile, "logins.json");
            var nssPath = profile;

            var passwordList = NSSHelper.GetFirefoxPasswords(loginsPath, nssPath);
            foreach (var passwordEntry in passwordList)
            {
                Console.WriteLine($"URL: {passwordEntry.Url}\nUsername: {passwordEntry.Username}\nPassword: {passwordEntry.Password}\n");
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }
}
