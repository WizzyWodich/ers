using System.Net.Http.Json;

namespace ERS.Web.Services;

public class SystemHealthService
{
    private readonly HttpClient _http;

    public SystemHealthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(bool IsApiAlive, bool IsDbConnected, bool IsDbEmpty, string? Error)> CheckHealthAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/health");
            
            if (!response.IsSuccessStatusCode)
            {
                return (true, false, false, "API доступен, но база данных лежит.");
            }

            var data = await response.Content.ReadFromJsonAsync<HealthResponse>();
            
            return (true, true, data!.IsEmpty, null);
        }
        catch (Exception)
        {
            return (false, false, false, "Сервер API недоступен.");
        }
    }

    private class HealthResponse
    {
        public string Status { get; set; } = "";
        public string Database { get; set; } = "";
        public bool IsEmpty { get; set; }
    }
}