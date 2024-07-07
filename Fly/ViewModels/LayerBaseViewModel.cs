using Mapsui.Layers;
using Mapsui.UI.Avalonia;
using System;
using System.ComponentModel;

namespace Fly.ViewModels;

public abstract class LayerBaseViewModel : ViewModelBase
{
    protected LayerBaseViewModel(
        string displayName
    )
    {
        DisplayName = displayName;
    }

    public string DisplayName { get; }

    private double _opacity;
    public double Opacity
    {
        get => _opacity;
        set => SetProperty(ref _opacity, value);
    }

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public LayerInfosBag CreateLayerInfosBag(ILayer layer, MapControl mapControl)
    {
        PropertyChangedEventHandler eventHandler = (s, e) => LayerViewModel_PropertyChanged((LayerBaseViewModel)s!, layer, e, mapControl);
        LayerInfosBag layerInfosBag = PrivateCreateLayerInfosBag(layer, eventHandler);
        return layerInfosBag;
    }
    protected virtual LayerInfosBag PrivateCreateLayerInfosBag(ILayer layer, PropertyChangedEventHandler eventHandler)
    {
        return new LayerInfosBag(layer, eventHandler, this);
    }

    private static void LayerViewModel_PropertyChanged(LayerBaseViewModel l, ILayer boundLayer, PropertyChangedEventArgs e, MapControl mapControl)
    {
        if (e.PropertyName == nameof(Opacity))
        {
            boundLayer.Opacity = l.Opacity;
            mapControl.InvalidateVisual();
        }
        else if (e.PropertyName == nameof(IsEnabled))
        {
            boundLayer.Enabled = l.IsEnabled;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
