using System;
using System.Diagnostics;
using BSOptimizerPro.Utils;
using Microsoft.Win32;

namespace BSOptimizerPro.Services
{
    public class TurboLoadService
    {
        private readonly MemoryService _memoryService = new MemoryService();

        public bool EnableTurboLoad()
        {
            try
            {
                // Limpeza de Memória Standby (ISLC style)
                _memoryService.ClearStandbyList();

                // 1. Manter Kernel na RAM (Evita paginação lenta)
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "DisablePagingExecutive", 1, RegistryValueKind.DWord);

                // 2. Expandir Cache do Sistema de Arquivos (LargeSystemCache)
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "LargeSystemCache", 1, RegistryValueKind.DWord);

                // 3. Otimização de I/O Page Lock Limit (Aumenta buffer de RAM para leitura de disco)
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "IoPageLockLimit", 983040, RegistryValueKind.DWord);

                // 4. NVIDIA Shader Cache Boost (10GB)
                RegistryHelper.SetRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\nvlddmkm\Global\NVTweak", "OglShaderCacheSpace", 10737418240, RegistryValueKind.DWord);

                // 5. Desativar Compressão de RAM (Libera CPU para carregar o jogo)
                RunPowerShell("Disable-MMAgent -mc");

                // 6. Desativar Defrag Agendado (Evita concorrência de I/O)
                RunCommand("schtasks", "/Change /TN \"\\Microsoft\\Windows\\Defrag\\ScheduledDefrag\" /Disable");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro Turbo Load: {ex.Message}");
                return false;
            }
        }

        private void RunPowerShell(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("powershell", $"-WindowStyle Hidden -Command \"{command}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi)?.WaitForExit();
        }

        private void RunCommand(string cmd, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi)?.WaitForExit();
        }
    }
}
