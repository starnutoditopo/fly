using Mapsui.Layers;
using Mapsui.Nts;
using System.ComponentModel;
using Fly.Widgets;
using Map = Mapsui.Map;
using System;
using System.Linq;

namespace Fly.ViewModels;

public class NewPointAddedMessage
{
    public NewPointAddedMessage(GeometryFeature point)
    {
        Point = point;
    }
    public GeometryFeature Point { get; }
}

public class FeaturesLayerViewModel : LayerBaseViewModel
{
    private class FeaturesLayerInfosBag : LayerInfosBag
    {
        private FlyEditingWidget? _flyEditingWidget;
        public FeaturesLayerInfosBag(
            WritableLayer layer,
            PropertyChangedEventHandler propertyChangedEventHandler,
            FeaturesLayerViewModel owner
        ) : base(layer, propertyChangedEventHandler, owner)
        {
        }

        private static FlyEditingWidget? GetEditingWidget(Map map)
        {
            FlyEditingWidget? flyEditingWidget = map.Widgets.OfType<FlyEditingWidget>().FirstOrDefault();
            if (flyEditingWidget != null)
            {
                return flyEditingWidget;
            }
            return null;
        }

        protected override void AssociatePrivate(Map map)
        {
            if(_flyEditingWidget != null)
            {
                throw new InvalidOperationException("Already associated!");
            }
            _flyEditingWidget = GetEditingWidget(map);
            if (_flyEditingWidget != null)
            {
                if (_flyEditingWidget.Layer == null)
                {
                    if (!(Layer is WritableLayer writableLayer))
                    {
                        throw new InvalidOperationException($"Only layers of type '{typeof(WritableLayer)}' can be associated.");
                    }

                    _flyEditingWidget.Layer = writableLayer;
                }
                else
                {
                    throw new InvalidOperationException("Layer is still associated.");
                }
            }
            else
            {
                throw new InvalidOperationException("FlyEditingWidget is null.");
            }
        }

        protected override void UnassociatePrivate(Map map)
        {
            if (_flyEditingWidget == null)
            {
                throw new InvalidOperationException("Layer is not associated.");
            }
            else
            {
                if (_flyEditingWidget.Layer != null)
                {
                    if (!(Layer is WritableLayer))
                    {
                        throw new InvalidOperationException($"Only layers of type '{typeof(WritableLayer)}' can be unassociated.");
                    }

                    if (_flyEditingWidget.Layer != Layer)
                    {
                        throw new InvalidOperationException("Layer is out of sync.");
                    }
                    _flyEditingWidget.Layer = null;
                }
                _flyEditingWidget = null;
            }
        }
    }

    public FeaturesLayerViewModel()
        : base("Features")
    {
    }

    protected override LayerInfosBag PrivateCreateLayerInfosBag(ILayer layer, PropertyChangedEventHandler eventHandler)
    {
        WritableLayer? writableLayer = layer as WritableLayer;
        if (writableLayer == null)
        {
            throw new InvalidOperationException($"Layer must be of type '{typeof(WritableLayer)}' and not null.");
        }
        return new FeaturesLayerInfosBag(writableLayer, eventHandler, this);
    }
}
