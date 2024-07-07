using System;

namespace Fly.ViewModels;

public class OpenStreetMapLayerViewModel : LayerBaseViewModel
{
    public OpenStreetMapLayerViewModel(
        string urlFormatter,
        string userAgent
        )
        : base("Open street map")
    {
        if (string.IsNullOrWhiteSpace(urlFormatter))
        {
            throw new ArgumentOutOfRangeException(nameof(urlFormatter), $"Parameter {nameof(urlFormatter)} is null or whitespace.");
        }
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            throw new ArgumentOutOfRangeException(nameof(userAgent), $"Parameter {nameof(userAgent)} is null or whitespace.");
        }

        UrlFormatter = urlFormatter;
        UserAgent = userAgent;
    }

    public string UrlFormatter { get; }
    public string UserAgent { get; }
}
