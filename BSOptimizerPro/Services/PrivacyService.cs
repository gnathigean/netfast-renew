using System;
using System.Diagnostics;
using BSOptimizerPro.Utils;
using Microsoft.Win32;

namespace BSOptimizerPro.Services
{
    public class PrivacyService
    {
        public bool StopSpyware()
        {
            try
            {
                // Parar serviços de diagnóstico e telemetria
                StopService("DiagTrack");
                StopService("dmwappushservice");

                // Desativar no registro
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                
                // Desativar tarefas agendadas (opcional, pode ser expandido)
                return true;
            }
            catch { return false; }
        }

        private void StopService(string serviceName)
        {
            Process.Start(new ProcessStartInfo("sc", $"stop {serviceName}") { CreateNoWindow = true, UseShellExecute = false })?.WaitForExit();
            Process.Start(new ProcessStartInfo("sc", $"config {serviceName} start= disabled") { CreateNoWindow = true, UseShellExecute = false })?.WaitForExit();
        }
    }
}
