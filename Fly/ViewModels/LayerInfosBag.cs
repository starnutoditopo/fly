using Mapsui;
using Mapsui.Layers;
using System.ComponentModel;

namespace Fly.ViewModels;

public class LayerInfosBag
{
    protected readonly LayerBaseViewModel _owner;
    public LayerInfosBag(
        ILayer layer,
        PropertyChangedEventHandler propertyChangedEventHandler,
        LayerBaseViewModel owner
    )
    {
        Layer = layer;
        _propertyChangedEventHandler = propertyChangedEventHandler;
        _owner = owner;
    }

    public ILayer Layer { get; }
    private readonly PropertyChangedEventHandler _propertyChangedEventHandler;

    public void Associate(Map map)
    {
        _owner.PropertyChanged += _propertyChangedEventHandler;
        AssociatePrivate(map);
    }
    protected virtual void AssociatePrivate(Map map)
    {
    }

    public void Unassociate(Map map)
    {
        UnassociatePrivate(map);
        _owner.PropertyChanged -= _propertyChangedEventHandler;
    }
    protected virtual void UnassociatePrivate(Map map)
    {
    }
}
