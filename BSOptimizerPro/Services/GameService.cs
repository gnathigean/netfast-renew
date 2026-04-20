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
                string path = PathService.GetBloodStrikeConfigPath();
                if (string.IsNullOrEmpty(path)) return false;

                var iniFiles = Directory.GetFiles(path, "*.ini", SearchOption.AllDirectories);
                foreach (var file in iniFiles)
                {
                    InjectPotatoConfig(file, extreme);
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

            // Detecção de GPU e Tweaks Específicos
            ApplyGPUTweaks();
            
            // Variáveis de Ambiente para Unity/Performance
            Environment.SetEnvironmentVariable("UNITY_DISABLE_HW_STATS", "1", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("__GL_THREADED_OPTIMIZATIONS", "1", EnvironmentVariableTarget.User);
        }

        private void ApplyGPUTweaks()
        {
            try
            {
                // Tweak Global (Independente da GPU)
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HWSchPriority", 1, RegistryValueKind.DWord);

                // Busca por IDs de Driver AMD no Registro
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}"))
                {
                    if (key != null)
                    {
                        foreach (string subkeyName in key.GetSubKeyNames())
                        {
                            if (subkeyName == "0000" || subkeyName == "0001" || subkeyName == "0002")
                            {
                                using (RegistryKey subkey = key.OpenSubKey(subkeyName, true))
                                {
                                    if (subkey == null) continue;
                                    
                                    string provider = subkey.GetValue("ProviderName") as string;
                                    if (provider != null && provider.Contains("Advanced Micro Devices"))
                                    {
                                        // Otimizações Específicas AMD
                                        subkey.SetValue("FlipQueueSize", new byte[] { 0x31, 0x00 }, RegistryValueKind.Binary); // 0 ou 1 (Anti-Lag)
                                        subkey.SetValue("ShaderCache", "32", RegistryValueKind.String);
                                        subkey.SetValue("KMD_EnableCrossFire", 0, RegistryValueKind.DWord);
                                    }
                                    else if (provider != null && provider.Contains("NVIDIA"))
                                    {
                                        // NVIDIA já é coberta pelo TurboLoadService via NVTweak, 
                                        // mas podemos adicionar ajustes globais de latência aqui se necessário.
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }
}
