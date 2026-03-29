using BusinessLayer;
using GlobalClasses;
using Microsoft.Win32;
using System;

namespace DVLD.Global_classes
{
    // Holds simple global helpers/state for the application (current user + credential storage).
    internal static class ClsGlobal
    {
        public static ClsUser CurrentUser;

        private const string FullKeyPath = @"HKEY_CURRENT_USER\Software\DVLD";
        private const string BaseKeyPath = @"Software\DVLD";
        private const string UserNameName = "UserName";
        private const string PasswordName = "Password";

        // Save or remove stored username/password based on IsChecked.
        public static void RememberUsernameAndPassword(bool IsChecked, string Password)
        {
            try
            {
                if (!IsChecked)
                {
                    using (RegistryKey Basekey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                    {
                        using (RegistryKey Key = Basekey.OpenSubKey(BaseKeyPath, true))
                        {
                            if (Key != null)
                            {
                                Key.DeleteValue(UserNameName, false);
                                Key.DeleteValue(PasswordName, false);
                            }
                        }
                    }
                    return;
                }

                Registry.SetValue(FullKeyPath, UserNameName, CurrentUser.UserName);
                Registry.SetValue(FullKeyPath, PasswordName, Password);
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return; }
        }

        // Read stored credentials from file into the provided ref parameters.
        public static bool GetStoredCredential(ref string UserName, ref string Password)
        {
            try
            {
                UserName = Registry.GetValue(FullKeyPath, UserNameName, string.Empty) as string;
                Password = Registry.GetValue(FullKeyPath, PasswordName, string.Empty) as string;

                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                { return true; }
                else
                { return false; }
            }
            catch (Exception e) { ClsLogger.Log(e); return false; }
        }
    }
}