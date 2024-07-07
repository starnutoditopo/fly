using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using System;
using System.IO;

namespace Fly.Tiling;

/// <summary>
/// A re-implementation of <see cref="Mapsui.Tiling.OpenStreetMap" /> accepting custom URL for tiling service (useful for self-hosted service).
/// </summary>
public static class OpenStreetMap
{    
    public static IPersistentCache<byte[]>? DefaultCache;

    private static readonly Attribution _openStreetMapAttribution = new Attribution("© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright");
    private const string DefaultUrlFormatter = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";

    public static TileLayer CreateTileLayer(
        string? urlFormatter = DefaultUrlFormatter,
        string? userAgent = null
    )
    {
        if (userAgent == null)
        {
            userAgent = "user-agent-of-" + Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        }

        if(urlFormatter == null)
        {
            urlFormatter = DefaultUrlFormatter;
        }

        return new TileLayer(CreateTileSource(urlFormatter, userAgent))
        {
            Name = "OpenStreetMap"
        };
    }

    private static HttpTileSource CreateTileSource(string urlFormatter, string userAgent)
    {
        GlobalSphericalMercator tileSchema = new GlobalSphericalMercator();
        Attribution openStreetMapAttribution = _openStreetMapAttribution;
        return new HttpTileSource(tileSchema, urlFormatter, null, null, "OpenStreetMap", DefaultCache, null, openStreetMapAttribution, userAgent);
    }

    internal static ILayer CreateTileLayer(object urlFormatter, object userAgent)
    {
        throw new NotImplementedException();
    }
}