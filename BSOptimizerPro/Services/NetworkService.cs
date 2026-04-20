using System;
using System.Diagnostics;
using BSOptimizerPro.Utils;
using Microsoft.Win32;

namespace BSOptimizerPro.Services
{
    public class NetworkService
    {
        public bool OptimizeNetwork()
        {
            try
            {
                // DNS e Winsock Reset
                RunNetsh("winsock reset");
                RunNetsh("int ip reset");
                RunCommand("ipconfig", "/flushdns");

                // TCP Tweaks no Registro
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "TcpAckFrequency", 1, RegistryValueKind.DWord);
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "TcpNoDelay", 1, RegistryValueKind.DWord);
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "TcpWindowSize", 64240, RegistryValueKind.DWord);
                
                return true;
            }
            catch { return false; }
        }

        private void RunNetsh(string args) => RunCommand("netsh", args);

        private void RunCommand(string cmd, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Verb = "runas" // Solicita admin se necessário
            };
            Process.Start(psi)?.WaitForExit();
        }
    }
}
