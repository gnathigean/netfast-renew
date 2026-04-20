using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace BSOptimizerPro.Services
{
    public static class HwidService
    {
        /// <summary>
        /// Gera um identificador único para a máquina baseado no UUID da placa mãe ou Volume Serial.
        /// </summary>
        public static string GetHwid()
        {
            try
            {
                string uuid = GetMotherboardUuid();
                if (!string.IsNullOrEmpty(uuid) && uuid != "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")
                {
                    return HashValue(uuid);
                }

                // Fallback para Volume Serial do C:
                return HashValue(GetVolumeSerial("C"));
            }
            catch
            {
                return "HWID_UNKNOWN";
            }
        }

        private static string GetMotherboardUuid()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["UUID"]?.ToString() ?? "";
                    }
                }
            }
            catch { }
            return "";
        }

        private static string GetVolumeSerial(string drive)
        {
            try
            {
                using (ManagementObject disk = new ManagementObject($"win32_logicaldisk.deviceid=\"{drive}:\""))
                {
                    disk.Get();
                    return disk["VolumeSerialNumber"]?.ToString() ?? "";
                }
            }
            catch { }
            return "NO_SERIAL";
        }

        private static string HashValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return "INVALID_DATA";
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString().Substring(0, 16).ToUpper(); // Retorna 16 caracteres para manter consistência
            }
        }
    }
}
