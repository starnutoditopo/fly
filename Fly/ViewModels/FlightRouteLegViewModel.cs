using Fly.Models;

namespace Fly.ViewModels;

public class FlightRouteLegViewModel : ViewModelBase, ITimelineItem
{
    public FlightRouteLegViewModel(FlightRouteLeg flightRouteLeg)
    {
        FlightRouteLeg = flightRouteLeg;
    }
    public FlightRouteLeg FlightRouteLeg { get; }
}
