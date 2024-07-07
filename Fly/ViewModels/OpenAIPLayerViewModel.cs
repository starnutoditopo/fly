using System;
using System.Collections.Generic;
using System.Linq;

namespace Fly.ViewModels;

public class OpenAIPLayerViewModel : LayerBaseViewModel
{
    public OpenAIPLayerViewModel(
        string apiKey,
        string urlFormatter,
        IEnumerable<string> serversList,
        string userAgent
    ) : base("Open AIP")
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
        ArgumentNullException.ThrowIfNull(serversList);
        ApiKey = apiKey;
        UrlFormatter = urlFormatter;
        UserAgent = userAgent;
        ServersList = serversList.ToList().AsReadOnly();
    }

    public string ApiKey { get; }
    public string UrlFormatter { get; }
    public IEnumerable<string> ServersList { get; }
    public string UserAgent { get; }
}
