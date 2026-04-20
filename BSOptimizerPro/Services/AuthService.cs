using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BSOptimizerPro.Services
{
    public class AuthService
    {
        private static readonly HttpClient _client = new HttpClient();
        private const string BaseUrl = "https://bs-optimizer-api.onrender.com";

        public async Task<(bool Success, string Message, int UserId)> LoginAsync(string username, string password)
        {
            try
            {
                var payload = new
                {
                    username = username,
                    password = password,
                    hwid = HwidService.GetHwid()
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{BaseUrl}/api/desktop/login", content);
                var result = await response.Content.ReadAsStringAsync();
                
                JObject json = JObject.Parse(result);
                bool success = json["sucesso"]?.Value<bool>() ?? false;
                string message = json["mensagem"]?.ToString() ?? "Erro desconhecido";
                int userId = json["user_id"]?.Value<int>() ?? 0;

                return (success, message, userId);
            }
            catch (Exception ex)
            {
                return (false, $"Erro de conexão: {ex.Message}", 0);
            }
        }

        public async Task<(bool HasLicense, int DaysRemaining)> CheckLicenseAsync(int userId)
        {
            try
            {
                var payload = new { user_id = userId };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{BaseUrl}/api/desktop/licenca", content);
                var result = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(result);
                bool hasLicense = json["tem_licenca"]?.Value<bool>() ?? false;
                int days = json["dias_restantes"]?.Value<int>() ?? 0;

                return (hasLicense, days);
            }
            catch
            {
                return (false, 0);
            }
        }

        public async Task<(bool Success, string Message)> ActivateKeyAsync(int userId, string key)
        {
            try
            {
                var payload = new { user_id = userId, key = key };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"{BaseUrl}/api/desktop/ativar", content);
                var result = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(result);
                bool success = json["sucesso"]?.Value<bool>() ?? false;
                string message = json["mensagem"]?.ToString() ?? "Erro ao ativar chave";

                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, $"Erro na ativação: {ex.Message}");
            }
        }
    }
}
