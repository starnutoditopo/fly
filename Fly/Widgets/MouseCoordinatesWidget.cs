using Mapsui.Extensions;
using Mapsui.Widgets.BoxWidgets;
using Mapsui;
using Mapsui.Projections;
using Mapsui.Widgets;

namespace Fly.Widgets;

//public class MouseCoordinatesWidgetRenderer : ISkiaWidgetRenderer
//{
//    public void Draw(SKCanvas canvas, Viewport viewport, IWidget widget, float layerOpacity)
//    {
//        TextBoxWidgetRenderer.DrawText(canvas, viewport, (MouseCoordinatesWidget)widget, layerOpacity);
//        // use all of the canvas
//        widget.Envelope = new MRect(0, 0, viewport.Width, viewport.Height);
//    }
//}


/// <summary>
/// Widget that shows actual mouse coordinates in a TextBox
/// </summary>
public class MouseCoordinatesWidget : TextBoxWidget
{
    public MouseCoordinatesWidget()
    {
        InputAreaType = InputAreaType.Map;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Bottom;
        Text = "Mouse Position";
    }

    public override bool OnPointerMoved(Navigator navigator, WidgetEventArgs e)
    {
        var worldPosition = navigator.Viewport.ScreenToWorld(e.Position);
        // update the Mouse position
        var lonLat = SphericalMercator.ToLonLat(worldPosition);
        Text = $"{lonLat.X:F3}; {lonLat.Y:F3}";
        return false;
    }
}
