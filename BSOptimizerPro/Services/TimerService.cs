using System;
using BSOptimizerPro.Utils;

namespace BSOptimizerPro.Services
{
    public class TimerService
    {
        /// <summary>
        /// Define a resolução do relógio do sistema para o mínimo possível (0.5ms).
        /// Reduz o input lag em jogos e melhora a precisão do agendamento de threads.
        /// </summary>
        public void SetHighResolution()
        {
            try
            {
                // Método Clássico (1ms)
                NativeMethods.TimeBeginPeriod(1);

                // Método de Baixo Nível (0.5ms = 5000 unidades de 100ns)
                uint currentRes;
                NativeMethods.NtSetTimerResolution(5000, true, out currentRes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao definir timer resolution: {ex.Message}");
            }
        }

        public void ResetResolution()
        {
            try
            {
                NativeMethods.TimeEndPeriod(1);
            }
            catch { }
        }
    }
}
