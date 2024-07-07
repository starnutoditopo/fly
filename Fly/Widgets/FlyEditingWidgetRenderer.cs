using Mapsui;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Widgets;

namespace Fly.Widgets
{
    public class FlyEditingWidgetRenderer : ISkiaWidgetRenderer
    {
        public void Draw(SkiaSharp.SKCanvas canvas, Viewport viewport, IWidget widget, float layerOpacity)
        {
            // use all of the canvas
            widget.Envelope = new MRect(0, 0, viewport.Width, viewport.Height);
        }
    }
}
