using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using SkiaSharp;
using Fly.Extensions;
using Fly.Features;
namespace Fly.StyleRenderers;
public class FlyStyleRenderer : ISkiaStyleRenderer
{
    private readonly SKPaint _paint = new()
    {
        IsAntialias = true,
        IsStroke = false,
        FakeBoldText = false,
        IsEmbeddedBitmapText = true
    };

    public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer, IFeature feature, IStyle style, IRenderCache renderCache, long iteration)
    {
        var myStyle = (FlyStyle)style;

        if (feature is RouteFeature routeFeature)
        {
            if (routeFeature.ViewModel.IsVisible)
            {
                if (routeFeature.Geometry is LineString linestring)
                {
                    for (int i = 1; i < linestring.Coordinates.Length; i++)
                    {
                        var previousCoordinate = linestring.Coordinates[i - 1];
                        var currentCoordinate = linestring.Coordinates[i];
                        var previousWorldPoint = previousCoordinate.ToMPoint();
                        var currentWorldPoint = currentCoordinate.ToMPoint();

                        var from = viewport.WorldToScreen(previousWorldPoint);
                        var to = viewport.WorldToScreen(currentWorldPoint);
                        var mid = new MPoint((to.X + from.X) / 2, (to.Y + from.Y) / 2);
                        //var color = new SKColor(255, 0, 0);
                        //var colored = new SKPaint() { Color = color, IsAntialias = true };

                        //canvas.Save();
                        //canvas.Translate((float)mid.X, (float)mid.Y);
                        //canvas.DrawCircle(0, 0, 15, colored);
                        //canvas.Restore();

                        //var text = myStyle.GetLabelText(feature);
                        var previousLonLat = SphericalMercator.ToLonLat(previousWorldPoint);
                        var currentLonLat = SphericalMercator.ToLonLat(currentWorldPoint);
                        //var distance = Mapsui.Utilities.Haversine.Distance(previousLonLat.X, previousLonLat.Y, currentLonLat.X, currentLonLat.Y);
                        var rhumbDistance = GeographyExtensions.GetRhumbDistance(previousLonLat.X, previousLonLat.Y, currentLonLat.X, currentLonLat.Y);
                        var rhumbBearing = GeographyExtensions.GetRhumbBearing(previousLonLat.X, previousLonLat.Y, currentLonLat.X, currentLonLat.Y);
                        var text = $"{rhumbBearing:0}°\n{rhumbDistance / 1000:0} Km";
                        var mustFitIn = (float)(to.Distance(from) + 10);
                        LabelDrawingHelper.DrawLabel(canvas, (float)mid.X, (float)mid.Y, myStyle, text, (float)layer.Opacity, renderCache, mustFitIn, _paint);
                    }
                    return true;
                }
            }
        }
        return false;
    }
}

