using System;
using System.Runtime.InteropServices;
using BSOptimizerPro.Utils;

namespace BSOptimizerPro.Services
{
    public class MemoryService
    {
        /// <summary>
        /// Limpa a lista de memória Standby do Windows, similar ao ISLC.
        /// Exige privilégios de administrador.
        /// </summary>
        public bool ClearStandbyList()
        {
            try
            {
                int size = Marshal.SizeOf(NativeMethods.MemoryPurgeStandbyList);
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.WriteInt32(ptr, NativeMethods.MemoryPurgeStandbyList);

                int result = NativeMethods.NtSetSystemInformation(
                    NativeMethods.SystemMemoryListInformation, 
                    ptr, 
                    size
                );

                Marshal.FreeHGlobal(ptr);

                return result == 0; // STATUS_SUCCESS
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar standby: {ex.Message}");
                return false;
            }
        }
    }
}
