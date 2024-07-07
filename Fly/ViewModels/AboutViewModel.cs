using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Fly.ViewModels;
public class AboutViewModel : ViewModelBase
{
    public AboutViewModel()
    {
        var version = Assembly.GetExecutingAssembly()?
          .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
          .InformationalVersion;
        if (string.IsNullOrWhiteSpace(version))
        {
            Version = "<unknown version>";
        }
        else
        {
            Version = version
                .Split('+')
                [0]
                .Split('-')
                [0];
        }
    }
    public string Product { get; } = Constants.PRODUCT_NAME;
    public string Version { get; }
    private const string GithubLinkUri = "https://github.com/starnutoditopo/Fly/";
    private const string OpenAipLinkUri = "https://www.openaip.net/";
    private const string OpenStreetMapUri = "https://www.openstreetmap.org/";
    private const string MapTilerSatelliteUri = "https://www.maptiler.com/";
    private const string MapsUiUri = "https://github.com/Mapsui/Mapsui/";
    private const string OpenRouteServiceUri = "https://openrouteservice.org/";

    public void OpenGithubLinkUri()
    {
        OpenBrowser(GithubLinkUri);
    }

    public void OpenOpenAipLinkUri()
    {
        OpenBrowser(OpenAipLinkUri);
    }
    public void OpenOpenStreetMapUri()
    {
        OpenBrowser(OpenStreetMapUri);
    }
    public void OpenMapTilerSatelliteUri()
    {
        OpenBrowser(MapTilerSatelliteUri);
    }
    public void OpenOpenRouteServiceUri()
    {
        OpenBrowser(OpenRouteServiceUri);
    }

    public void OpenMapsUiUri()
    {
        OpenBrowser(MapsUiUri);
    }

    /// <summary>
    /// Opens the browser.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <remarks>See: https://github.com/dotnet/runtime/issues/21798</remarks>
    private bool OpenBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}") { CreateNoWindow = true });
            return true;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
            return true;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
            return true;
        }
        return false;
    }
}
