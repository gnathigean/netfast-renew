using Microsoft.Win32;
using System;

namespace BSOptimizerPro.Utils
{
    public static class RegistryHelper
    {
        public static bool SetRegistryValue(string keyPath, string valueName, object value, RegistryValueKind kind)
        {
            try
            {
                RegistryKey root = keyPath.StartsWith("HKEY_LOCAL_MACHINE") ? Registry.LocalMachine : Registry.CurrentUser;
                string subKeyPath = keyPath.Substring(keyPath.IndexOf('\\') + 1);

                using (RegistryKey key = root.CreateSubKey(subKeyPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue(valueName, value, kind);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao definir registro: {ex.Message}");
            }
            return false;
        }

        public static bool DeleteRegistryValue(string keyPath, string valueName)
        {
            try
            {
                RegistryKey root = keyPath.StartsWith("HKEY_LOCAL_MACHINE") ? Registry.LocalMachine : Registry.CurrentUser;
                string subKeyPath = keyPath.Substring(keyPath.IndexOf('\\') + 1);

                using (RegistryKey key = root.OpenSubKey(subKeyPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(valueName, false);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}
