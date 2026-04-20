using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSOptimizerPro.Services
{
    public class GameSettingsService
    {
        private string _configPath;

        public GameSettingsService()
        {
            _configPath = PathService.GetBloodStrikeConfigPath();
        }

        public void ApplySettings(Dictionary<string, string> settings)
        {
            if (string.IsNullOrEmpty(_configPath)) return;
            
            string file = Path.Combine(_configPath, "GameUserSettings.ini");
            if (!File.Exists(file)) return;

            var lines = File.ReadAllLines(file).ToList();
            
            foreach (var setting in settings)
            {
                bool found = false;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith(setting.Key + "="))
                    {
                        lines[i] = $"{setting.Key}={setting.Value}";
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    lines.Add($"{setting.Key}={setting.Value}");
                }
            }

            File.WriteAllLines(file, lines);
        }

        public void SetCompetitiveConfig()
        {
            var competitive = new Dictionary<string, string>
            {
                { "FrameRateLimit", "0.000000" }, // FPS Ilimitado
                { "FullscreenMode", "0" },        // Tela Cheia Exclusiva
                { "LastConfirmedFullscreenMode", "0" },
                { "PreferredFullscreenMode", "0" },
                { "sg.ShadowQuality", "0" },      // Sombras Desativadas
                { "sg.PostProcessQuality", "0" }, // Sem Pós-Processamento
                { "sg.TextureQuality", "1" },      // Texturas Médias (Vantagem Visual)
                { "sg.EffectsQuality", "0" },     // Efeitos Mínimos
                { "sg.FoliageQuality", "0" },     // Sem Grama/Folhagem
                { "bUseVSync", "False" }          // VSync Desativado (Zero Delay)
            };

            ApplySettings(competitive);
        }
    }
}
