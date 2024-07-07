using System.Net.Http;

namespace Fly.Services;

public partial class OpenRouteService
{
    private readonly HttpClient _httpClient;
    private readonly ISettingsService _settingsService;

    public OpenRouteService(
        HttpClient httpClient,
        ISettingsService settingsService
        )
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }
}
