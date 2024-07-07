using Mapsui.Styles;
using Mapsui.Utilities;
using Color = Mapsui.Styles.Color;
using Mapsui.Extensions;

namespace Fly.StyleRenderers;


public static class FlyMarkerStyles
{
    private static int LoadSvgId(string relativePathToEmbeddedResource)
    {
        var assembly = typeof(FlyMarkerStyles).Assembly;
        var fullName = assembly.GetFullName(relativePathToEmbeddedResource);

        if (!BitmapRegistry.Instance.TryGetBitmapId(relativePathToEmbeddedResource, out var bitmapId))
        {
            var result = assembly.GetManifestResourceStream(fullName).LoadSvgPicture();
            if (result != null)
            {
                bitmapId = BitmapRegistry.Instance.Register(result, fullName);
                return bitmapId;
            }
        }

        return bitmapId;
    }

    public static SymbolStyle CreatePinStyle(bool isEnabled, Color? pinColor = null, double symbolScale = 1.0)
    {
        int bitmapId = LoadSvgId("Assets.pin.svg");

        return new SymbolStyle()
        {
            BitmapId = bitmapId,
            SymbolOffset = new RelativeOffset(0.0, 0.5),
            SymbolScale = symbolScale,
            BlendModeColor = pinColor ?? Color.FromArgb(255, 57, 115, 199),
            Enabled = isEnabled,
        };
    }
}