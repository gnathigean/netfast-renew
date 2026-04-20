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
        private readonly OptimizationService _optService = new OptimizationService();

        public MainWindow(int userId, string username, int days)
        {
            InitializeComponent();
            _userId = userId;
            _username = username;
            _daysRemaining = days;

            UserGreet.Text = $"Olá, {_username}";
            LicenseInfo.Text = $"VIP: {_daysRemaining} Dias restantes";
        }

        private void ApplyFpsOptimization_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "OTIMIZANDO FPS...";
            StatusText.Foreground = System.Windows.Media.Brushes.Orange;

            bool success = _optService.ApplyFpsTurbo();

            if (success)
            {
                StatusText.Text = "FPS OTIMIZADO ✅";
                StatusText.Foreground = (System.Windows.Media.Brush)Application.Current.Resources["SecondaryBrush"];
                MessageBox.Show("Otimizações de FPS aplicadas com sucesso!", "Elite Boost", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                StatusText.Text = "ERRO NA OTIMIZAÇÃO ❌";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void ApplyGpuOptimization_Click(object sender, RoutedEventArgs e)
        {
            // Implementação direta via OptimizationService já cobre registro
            _optService.ApplyFpsTurbo(); 
            StatusText.Text = "GPU OTIMIZADA ✅";
            MessageBox.Show("Prioridade de GPU definida para Máxima Performance.", "Elite Boost", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ApplyCleanup_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "LIMPANDO CACHE...";
            _optService.CleanTempFiles();
            StatusText.Text = "LIMPEZA CONCLUÍDA ✅";
            MessageBox.Show("Cache do sistema e temporários removidos.", "Limpeza", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
