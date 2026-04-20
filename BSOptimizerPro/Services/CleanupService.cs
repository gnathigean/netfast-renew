using System;
using System.IO;

namespace BSOptimizerPro.Services
{
    public class CleanupService
    {
        public void SmartCleanup()
        {
            try
            {
                // Limpar Temp do Windows
                CleanDirectory(Path.GetTempPath());
                CleanDirectory(@"C:\Windows\Temp");

                // Limpar Cache da Unity / Jogos
                string localLoadDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "NetEase");
                CleanDirectory(localLoadDir);

                // Limpeza de Shader Cache da NVIDIA / AMD (cuidado: pode ser acessado pelo driver)
                // É melhor focar em arquivos do jogo para prevenir stuttering na primeira carga.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na limpeza: {ex.Message}");
            }
        }

        private void CleanDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                try { file.Delete(); } catch { } // Ignora arquivos em uso
            }
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                try { subDir.Delete(true); } catch { }
            }
        }
    }
}
