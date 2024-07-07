using Fly.Models;

namespace Fly.ViewModels;

public class RouteViewModel : RouteBaseViewModel
{
    private int? _editingPointIndex;
    public int? EditingPointIndex
    {
        get=> _editingPointIndex;
        set => SetProperty(ref _editingPointIndex, value);
    }

    public virtual CoordinateModel[]? CoordinatesAtDraggingStart { get; set; }
    public virtual CoordinateModel? MouseStartDraggingCoordinate { get; set; }
}
