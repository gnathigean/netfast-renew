using System;
using System.Windows;
using BSOptimizerPro.Services;

namespace BSOptimizerPro
{
    public partial class MainWindow : Window
    {
        private readonly int _userId;
        private readonly string _username;
        private readonly int _daysRemaining;

        // Serviços
        private readonly NetworkService _netService = new NetworkService();
        private readonly AudioService _audioService = new AudioService();
        private readonly PrivacyService _privacyService = new PrivacyService();
        private readonly GameService _gameService = new GameService();
        private readonly SnapTapService _snapTap = new SnapTapService();
        private readonly TurboLoadService _turboService = new TurboLoadService();
        private OverlayWindow _overlay;

        public MainWindow(int userId, string username, int days)
        {
            InitializeComponent();
            _userId = userId;
            _username = username;
            _daysRemaining = days;

            UserGreet.Text = $"Olá, {_username}";
            LicenseInfo.Text = $"VIP: {_daysRemaining} Dias restantes";

            DetectHardware();
        }

        private void DetectHardware()
        {
            try
            {
                string gpu = "Desconhecida";
                using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_VideoController"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        gpu = obj["Name"].ToString();
                        break;
                    }
                }
                string path = PathService.GetBloodStrikeConfigPath() != null ? "DETECTADO" : "NÃO ENCONTRADO";
                HardwareInfo.Text = $"GPU: {gpu}\nBLOOD STRIKE: {path}";
            }
            catch { HardwareInfo.Text = "Hardware: Falha na detecção"; }
        }

        // --- DASHBOARD ---
        private void QuickOptimize_Click(object sender, RoutedEventArgs e)
        {
            ApplyPerf_Click(null, null);
            ApplyNetwork_Click(null, null);
            ApplyAudio_Click(null, null);
            ApplyPrivacy_Click(null, null);
            GlobalStatus.Text = "OTIMIZAÇÃO ELITE APLICADA ✅";
        }

        // --- PERFORMANCE ---
        private void ApplyPerf_Click(object sender, RoutedEventArgs e)
        {
            if (ChkFpsTurbo.IsChecked == true) _gameService.ApplyEngineTweaks();
            if (ChkPotato.IsChecked == true) _gameService.ApplyPotatoMode(true);
            if (ChkTurboLoad.IsChecked == true) _turboService.EnableTurboLoad();
            MessageBox.Show("Otimizações de performance, gráficos e carregamento turbo aplicadas!", "Elite Boost");
        }

        private void ApplyEngine_Click(object sender, RoutedEventArgs e)
        {
            _gameService.ApplyEngineTweaks();
            MessageBox.Show("Tweaks de engine Unreal/Unity aplicados com sucesso.");
        }

        // --- NETWORK & AUDIO ---
        private void ApplyNetwork_Click(object sender, RoutedEventArgs e)
        {
            _netService.OptimizeNetwork();
            MessageBox.Show("DNS limpo e roteamento TCP otimizado!");
        }

        private void ApplyAudio_Click(object sender, RoutedEventArgs e)
        {
            _audioService.OptimizeAudio();
            MessageBox.Show("Modo de Áudio Pro ativado!");
        }

        // --- SYSTEM & SOCD ---
        private void ToggleSnapTap_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSnapTap.IsChecked == true)
            {
                _snapTap.Start();
                BtnSnapTap.Content = "ON";
            }
            else
            {
                _snapTap.Stop();
                BtnSnapTap.Content = "OFF";
            }
        }

        private void ToggleOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (BtnOverlay.IsChecked == true)
            {
                _overlay = new OverlayWindow();
                _overlay.Show();
                BtnOverlay.Content = "ON";
            }
            else
            {
                _overlay?.Close();
                BtnOverlay.Content = "OFF";
            }
        }

        private void BtnLockOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (_overlay == null) return;
            
            bool isLocked = BtnLockOverlay.IsChecked == false; // Se não tá checado, tá travado (🔓) 
            _overlay.SetLocked(isLocked);
            BtnLockOverlay.Content = isLocked ? "🔓" : "🔒";
        }

        private void ApplyPrivacy_Click(object sender, RoutedEventArgs e)
        {
            _privacyService.StopSpyware();
            MessageBox.Show("Telemetria do Windows desativada.");
        }

        private void ResetSystem_Click(object sender, RoutedEventArgs e)
        {
            // Lógica de Rollback (Pode ser implementada com backup do registro)
            MessageBox.Show("Funcionalidade de Rollback será configurada no próximo update.");
        }

        // --- WINDOW CONTROLS ---
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _snapTap.Stop();
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _snapTap.Stop();
            Application.Current.Shutdown();
        }
    }
}
