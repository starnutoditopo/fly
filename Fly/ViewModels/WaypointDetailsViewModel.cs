using Fly.Models;

namespace Fly.ViewModels;

public class WaypointDetailsViewModel : ViewModelBase, ITimelineItem
{
    public WaypointDetailsViewModel(CoordinateBaseViewModel coordinate)
    {
        Coordinate = coordinate;
    }
    public CoordinateBaseViewModel Coordinate { get; }
}
