using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui.Tiling.Layers;

namespace Fly.Tiling;

public static class MapTilerSatellite
{
    public static IPersistentCache<byte[]>? DefaultCache = null;

    private static readonly Attribution attribution = new Attribution("© MapTiler", "https://www.maptiler.com/satellite/");

    public static TileLayer CreateTileLayer(
        string apiKey,
        string urlFormatter,
        string userAgent
    )
    {
        HttpTileSource httpTileSource = CreateTileSource(userAgent, urlFormatter, apiKey);
        TileLayer tileLayer = new TileLayer(httpTileSource)
        {
            Name = "MapTilerSatellite"
        };

        return tileLayer;
    }

    private static HttpTileSource CreateTileSource(
         string userAgent,
         string urlFormatter,
         string apiKey
     )
    {
        GlobalSphericalMercator tileSchema = new GlobalSphericalMercator();
        return new HttpTileSource(tileSchema, urlFormatter, null, apiKey, "MapTiler", DefaultCache, null, attribution, userAgent);
    }
}
