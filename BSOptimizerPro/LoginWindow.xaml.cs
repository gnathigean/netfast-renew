using System;
using System.Windows;
using BSOptimizerPro.Services;

namespace BSOptimizerPro
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService = new AuthService();
        private int _userId;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Preencha todos os campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var (success, message, userId) = await _authService.LoginAsync(user, pass);

            if (success)
            {
                _userId = userId;
                CheckLicense();
            }
            else
            {
                MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CheckLicense()
        {
            var (hasLicense, days) = await _authService.CheckLicenseAsync(_userId);

            if (hasLicense)
            {
                MainWindow main = new MainWindow(_userId, UsernameBox.Text, days);
                main.Show();
                this.Close();
            }
            else
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                ActivationPanel.Visibility = Visibility.Visible;
            }
        }

        private async void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            string key = KeyBox.Text.Trim();
            if (string.IsNullOrEmpty(key)) return;

            var (success, message) = await _authService.ActivateKeyAsync(_userId, key);

            if (success)
            {
                MessageBox.Show(message, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckLicense();
            }
            else
            {
                MessageBox.Show(message, "Chave Inválida", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ActivationPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
        }
    }
}
