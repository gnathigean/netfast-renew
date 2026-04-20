using System;
using System.IO;
using System.Text.RegularExpressions;
using BSOptimizerPro.Utils;
using Microsoft.Win32;

namespace BSOptimizerPro.Services
{
    public class GameService
    {
        public bool ApplyPotatoMode(bool extreme = false)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string[] possiblePaths = {
                    Path.Combine(localAppData, "BloodStrike", "Saved", "Config", "WindowsNoEditor"),
                    Path.Combine(localAppData, "ProjectBlood", "Saved", "Config", "WindowsNoEditor")
                };

                foreach (var path in possiblePaths)
                {
                    if (Directory.Exists(path))
                    {
                        var iniFiles = Directory.GetFiles(path, "GameUserSettings.ini", SearchOption.AllDirectories);
                        foreach (var file in iniFiles)
                        {
                            InjectPotatoConfig(file, extreme);
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }

        private void InjectPotatoConfig(string filePath, bool extreme)
        {
            string content = File.ReadAllText(filePath);

            // Qualidade Gráfica no Zero
            content = Regex.Replace(content, @"sg\.ShadowQuality=[0-9]+", "sg.ShadowQuality=0");
            content = Regex.Replace(content, @"sg\.FoliageQuality=[0-9]+", "sg.FoliageQuality=0");
            content = Regex.Replace(content, @"sg\.PostProcessQuality=[0-9]+", "sg.PostProcessQuality=0");
            content = Regex.Replace(content, @"sg\.EffectsQuality=[0-9]+", "sg.EffectsQuality=0");
            content = Regex.Replace(content, @"sg\.TextureQuality=[0-9]+", extreme ? "sg.TextureQuality=0" : "sg.TextureQuality=1");

            // Resolução Interna (Dynamic Resolution Scale) - Se o usuário quiser o modo EXTREME
            if (extreme)
            {
                content = Regex.Replace(content, @"sg\.ResolutionQuality=[0-9.]+", "sg.ResolutionQuality=50.000000");
            }

            File.WriteAllText(filePath, content);
        }

        public void ApplyEngineTweaks()
        {
            // Otimização de Mouse Raw Input
            RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\mouclass\Parameters", "MouseDataQueueSize", 20, RegistryValueKind.DWord);
            
            // Desativar Fullscreen Optimizations (FSO)
            RegistryHelper.SetRegistryValue(@"HKEY_CURRENT_USER\System\GameConfigStore", "GameDVR_FSEBehavior", 2, RegistryValueKind.DWord);
            
            // Variáveis de Ambiente para Unity/Performance
            Environment.SetEnvironmentVariable("UNITY_DISABLE_HW_STATS", "1", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("__GL_THREADED_OPTIMIZATIONS", "1", EnvironmentVariableTarget.User);
        }
    }
}
