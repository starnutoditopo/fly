using System;

namespace Fly.ViewModels;

public class LayerType
{
    private readonly Func<LayerBaseViewModel> _createLayer;
    private readonly Func<bool> _canCreateLayer;
    public LayerType(
        string displayName,
        Func<LayerBaseViewModel> createLayer,
        Func<bool> canCreateLayer
    )
    {
        ArgumentNullException.ThrowIfNull(createLayer);
        ArgumentNullException.ThrowIfNull(canCreateLayer);

        DisplayName = displayName;
        _createLayer = createLayer;
        _canCreateLayer = canCreateLayer;
    }
    public string DisplayName { get; }

    public LayerBaseViewModel CreateLayer()
    {
        if (!_canCreateLayer())
        {
            throw new InvalidOperationException("Cannot create layer.");
        }
        return _createLayer();
    }

    public bool CanCreateLayer => _canCreateLayer();
}
