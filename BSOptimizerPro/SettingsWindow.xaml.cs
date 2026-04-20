using System.Collections.Generic;
using System.Windows;
using BSOptimizerPro.Services;

namespace BSOptimizerPro
{
    public partial class SettingsWindow : Window
    {
        private readonly GameSettingsService _settingsService = new GameSettingsService();

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PresetElite_Click(object sender, RoutedEventArgs e)
        {
            _settingsService.SetCompetitiveConfig();
            MessageBox.Show("Preset Elite (Low Render, Max Visibility) aplicado no GameUserSettings.ini!", "Sucesso");
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var config = new Dictionary<string, string>();

            // Resolução
            string resText = (ComboRes.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            if (!string.IsNullOrEmpty(resText) && resText.Contains("X"))
            {
                var parts = resText.Split(' ')[0]; // Pega a primeira parte antes do X (ex: 1920) mas é melhor fazer regex ou string real
                // Simplificação (num cenário ideal usaria regex)
                if (resText.Contains("1920")) { config["ResolutionSizeX"] = "1920"; config["ResolutionSizeY"] = "1080"; }
                else if (resText.Contains("1600")) { config["ResolutionSizeX"] = "1600"; config["ResolutionSizeY"] = "900"; }
                else if (resText.Contains("2560")) { config["ResolutionSizeX"] = "2560"; config["ResolutionSizeY"] = "1440"; }
                else if (resText.Contains("1280")) { config["ResolutionSizeX"] = "1280"; config["ResolutionSizeY"] = "720"; }
                else if (resText.Contains("1440")) { config["ResolutionSizeX"] = "1440"; config["ResolutionSizeY"] = "1080"; }
            }

            // Frame Rate
            string hzText = (ComboHz.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            if (hzText.Contains("Ilimitado")) config["FrameRateLimit"] = "0.000000";
            else if (hzText.Contains("60")) config["FrameRateLimit"] = "60.000000";
            else if (hzText.Contains("144")) config["FrameRateLimit"] = "144.000000";
            else if (hzText.Contains("240")) config["FrameRateLimit"] = "240.000000";
            else if (hzText.Contains("360")) config["FrameRateLimit"] = "360.000000";

            // Window Mode
            string winMode = (ComboWindow.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            if (winMode.Contains("Exclusiva")) config["FullscreenMode"] = "0";
            else if (winMode.Contains("Bordas")) config["FullscreenMode"] = "1";
            else config["FullscreenMode"] = "2";

            // Advanced Graphics
            config["sg.TextureQuality"] = ComboTextura.SelectedIndex == 0 ? "0" : (ComboTextura.SelectedIndex == 1 ? "1" : "2");
            config["sg.ShadowQuality"] = ChkShadows.IsChecked == true ? "1" : "0";
            config["sg.FoliageQuality"] = ChkFoliage.IsChecked == true ? "1" : "0";
            config["sg.PostProcessQuality"] = ChkPostProcess.IsChecked == true ? "1" : "0";
            config["bUseVSync"] = ChkVsync.IsChecked == true ? "True" : "False";

            _settingsService.ApplySettings(config);
            
            MessageBox.Show("Configurações salvas e injetadas com sucesso! Abra o jogo para testar.", "Personalizador Netfast");
            this.Close();
        }
    }
}
