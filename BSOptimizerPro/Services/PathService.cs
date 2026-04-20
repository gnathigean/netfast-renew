using System;
using System.IO;
using Microsoft.Win32;

namespace BSOptimizerPro.Services
{
    public static class PathService
    {
        public static string GetBloodStrikeConfigPath()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            // 1. Caminho Standalone (Launcher Oficial)
            string standalonePath = Path.Combine(localAppData, "BloodStrike", "Saved", "Config", "WindowsNoEditor");
            if (Directory.Exists(standalonePath)) return standalonePath;

            // 2. Caminho LocalLow (Alguns launchers da NetEase usam aqui)
            string localLow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "NetEase", "BloodStrike", "Saved", "Config", "WindowsNoEditor");
            if (Directory.Exists(localLow)) return localLow;

            // 3. Caminho Steam
            string steamPath = GetSteamGamePath("BloodStrike");
            if (!string.IsNullOrEmpty(steamPath))
            {
                string steamConfig = Path.Combine(steamPath, "Saved", "Config", "WindowsNoEditor");
                if (Directory.Exists(steamConfig)) return steamConfig;
            }

            return null;
        }

        private string GetSteamGamePath(string gameName)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                {
                    if (key != null)
                    {
                        string steamPath = key.GetValue("SteamPath") as string;
                        if (!string.IsNullOrEmpty(steamPath))
                        {
                            string gamePath = Path.Combine(steamPath.Replace("/", "\\"), "steamapps", "common", gameName);
                            if (Directory.Exists(gamePath)) return gamePath;
                        }
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
