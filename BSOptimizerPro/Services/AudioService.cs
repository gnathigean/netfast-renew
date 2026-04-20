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

                // Forçar Timer de Sistema (1ms) para sincronização de áudio/video
                NativeMethods.TimeBeginPeriod(1);

                return true;
            }
            catch { return false; }
        }
    }
}
