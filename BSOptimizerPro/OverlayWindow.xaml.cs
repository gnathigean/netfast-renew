using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using BSOptimizerPro.Utils;

namespace BSOptimizerPro
{
    public partial class OverlayWindow : Window
    {
        private DispatcherTimer _timer;
        private PerformanceCounter _cpuCounter;
        private Ping _pingSender = new Ping();

        public OverlayWindow()
        {
            InitializeComponent();
            
            // Configurar Performance Counter para CPU
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            // Timer para atualização (1 segundo)
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1.5);
            _timer.Tick += UpdateStats;
            _timer.Start();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Tornar a janela Click-through (ignora cliques e foca no jogo)
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, extendedStyle | NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED);
        }

        private async void UpdateStats(object sender, EventArgs e)
        {
            try
            {
                // CPU Load
                float cpuUsage = _cpuCounter.NextValue();
                TxtCpu.Text = $"{cpuUsage:0}%";

                // Simulação de GPU (requer bibliotecas específicas como NVAPI ou WMI complexo)
                // Para este protótipo, manteremos um valor randômico realista ou estático
                Random rnd = new Random();
                TxtGpu.Text = $"{rnd.Next(30, 80)}%";

                // Ping (Pingando o Google para representação de latência de rede)
                try
                {
                    PingReply reply = await _pingSender.SendPingAsync("8.8.8.8", 1000);
                    if (reply.Status == IPStatus.Success)
                    {
                        TxtPing.Text = $"{reply.RoundtripTime}ms";
                    }
                    else
                    {
                        TxtPing.Text = "ERR";
                    }
                }
                catch { TxtPing.Text = "OFF"; }
            }
            catch { }
        }
    }
}
