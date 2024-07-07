using System;

namespace Fly.ViewModels;

public class MapTilerSatelliteLayerViewModel : LayerBaseViewModel
{
    public MapTilerSatelliteLayerViewModel(
        string apiKey,
        string urlFormatter,
        string userAgent
        )
        : base("MapTiler satellite")
    {
        if (string.IsNullOrWhiteSpace(urlFormatter))
        {
            throw new ArgumentOutOfRangeException(nameof(urlFormatter), $"Parameter {nameof(urlFormatter)} is null or whitespace.");
        }
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            throw new ArgumentOutOfRangeException(nameof(userAgent), $"Parameter {nameof(userAgent)} is null or whitespace.");
        }
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentOutOfRangeException(nameof(apiKey), $"Parameter {nameof(apiKey)} is null or whitespace.");
        }
        ApiKey = apiKey;
        UrlFormatter = urlFormatter;
        UserAgent = userAgent;
    }

    public string ApiKey { get; }
    public string UrlFormatter { get; }
    public string UserAgent { get; }
}
