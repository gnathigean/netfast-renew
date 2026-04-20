using BSOptimizerPro.Utils;
using Microsoft.Win32;

namespace BSOptimizerPro.Services
{
    public class AudioService
    {
        public bool OptimizeAudio()
        {
            try
            {
                // Desativar Audio Ducking (diminuir volume do jogo em chamadas)
                RegistryHelper.SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Multimedia\Audio", "UserDuckingPreference", 3, RegistryValueKind.DWord);
                
                // Latência de Áudio
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio", "GPU Priority", 8, RegistryValueKind.DWord);
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio", "Priority", 6, RegistryValueKind.DWord);
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio", "Scheduling Category", "High", RegistryValueKind.String);
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Audio", "SFIO Priority", "High", RegistryValueKind.String);

                // Desativar latência dinâmica e filtros de surround que "sujam" os passos
                RegistryHelper.SetRegistryValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Audio", "DisableDynamicLatency", 1, RegistryValueKind.DWord);
                
                // Forçar Timer de Sistema (1ms) para sincronização de áudio/video
                NativeMethods.TimeBeginPeriod(1);

                return true;
            }
            catch { return false; }
        }
    }
}
