using System;
using System.IO;
using System.Text.RegularExpressions;
using BSOptimizerPro.Utils;
using Microsoft.Win32;

namespace BSOptimizerPro.Services
{
    public class OptimizationService
    {
        public bool ApplyFpsTurbo()
        {
            try
            {
                ApplyRegistryTweaks();
                OptimizeGameConfigs();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ApplyRegistryTweaks()
        {
            // GPU Priority para Games
            RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "GPU Priority", 8, RegistryValueKind.DWord);
            RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Priority", 6, RegistryValueKind.DWord);
            RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Scheduling Category", "High", RegistryValueKind.String);
            
            // Game Mode / Game DVR Off
            RegistryHelper.SetRegistryValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0, RegistryValueKind.DWord);
            
            // System Responsiveness
            RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0, RegistryValueKind.DWord);
        }

        private void OptimizeGameConfigs()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string[] possiblePaths = {
                Path.Combine(localAppData, "BloodStrike"),
                Path.Combine(localAppData, "ProjectBlood") 
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    var iniFiles = Directory.GetFiles(path, "*.ini", SearchOption.AllDirectories);
                    foreach (var file in iniFiles)
                    {
                        OptimizeIniFile(file);
                    }
                }
            }
        }

        private void OptimizeIniFile(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                
                // Desbloqueio de FPS e VSync
                content = Regex.Replace(content, @"FrameRateLimit=[0-9.]+", "FrameRateLimit=0.000000");
                content = Regex.Replace(content, @"MaxFPS=[0-9]+", "MaxFPS=0");
                content = Regex.Replace(content, @"bUseVSync=(True|true|False|false)", "bUseVSync=False");
                content = Regex.Replace(content, @"t\.MaxFPS=[0-9.]+", "t.MaxFPS=0");

                File.WriteAllText(filePath, content);
            }
            catch { }
        }

        public void CleanTempFiles()
        {
            string tempPath = Path.GetTempPath();
            DeleteDirectoryContents(tempPath);
            
            string prefetchPath = @"C:\Windows\Prefetch";
            DeleteDirectoryContents(prefetchPath);
        }

        private void DeleteDirectoryContents(string path)
        {
            if (!Directory.Exists(path)) return;

            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                try { file.Delete(); } catch { }
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try { dir.Delete(true); } catch { }
            }
        }
    }
}
