using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Tiling.Layers;
using System.Collections.Generic;

namespace Fly.Tiling;

public static class OpenAip
{
    public static IPersistentCache<byte[]>? DefaultCache = null;

    private static readonly Attribution attribution = new Attribution("© OpenAIP", "https://www.openaip.net");

    public static TileLayer CreateTileLayer(
        string apiKey,
        string urlFormatter,
        IEnumerable<string> serversList,
        string userAgent
    )
    {
        HttpTileSource httpTileSource = CreateTileSource(userAgent, urlFormatter, serversList, apiKey);
        TileLayer tileLayer = new TileLayer(httpTileSource)
        {
            Name = "OpenAip"
        };

        return tileLayer;
    }

    private static HttpTileSource CreateTileSource(
        string userAgent,
        string urlFormatter,
        IEnumerable<string> serversList,
        string apiKey
    )
    {
        GlobalSphericalMercator tileSchema = new GlobalSphericalMercator();
        return new HttpTileSource(tileSchema, urlFormatter, serversList, apiKey, "OpenAip", DefaultCache, null, attribution, userAgent);
    }
}