using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
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

        public async Task<int> GetPingAsync(string address)
        {
            try
            {
                using (Ping pingSender = new Ping())
                {
                    PingOptions options = new PingOptions { DontFragment = true };
                    string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
                    int timeout = 120;
                    PingReply reply = await pingSender.SendPingAsync(address, timeout, buffer, options);
                    
                    if (reply.Status == IPStatus.Success)
                    {
                        return (int)reply.RoundtripTime;
                    }
                }
            }
            catch { }
            return 999;
        }

        public bool SetDns(string primary, string secondary)
        {
            try
            {
                // Descobrir a interface ativa
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up && 
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        RunNetsh($"interface ip set dns \"{ni.Name}\" static {primary}");
                        if (!string.IsNullOrEmpty(secondary))
                            RunNetsh($"interface ip add dns \"{ni.Name}\" {secondary} index=2");
                    }
                }
                RunCommand("ipconfig", "/flushdns");
                return true;
            }
            catch { return false; }
        }
    }
}
