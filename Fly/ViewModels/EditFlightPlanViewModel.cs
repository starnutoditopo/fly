using Avalonia.Input.Platform;
using Fly.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Fly.ViewModels;

public class EditFlightPlanViewModel : ViewModelBase
{
    public EditFlightPlanViewModel(
        IClipboard clipboard,
        FlightPlanBaseViewModel flightPlan,
        ObservableCollection<PlaneBaseViewModel> availablePlanes,
        ObservableCollection<RouteBaseViewModel> availableRoutes
        )
    {
        FlightPlan = flightPlan;
        AvailablePlanes = availablePlanes;
        AvailableRoutes = availableRoutes;
        _clipboard = clipboard;

    }

    public FlightPlanBaseViewModel FlightPlan { get; }

    public ObservableCollection<PlaneBaseViewModel> AvailablePlanes { get; }
    public ObservableCollection<RouteBaseViewModel> AvailableRoutes { get; }

    private readonly IClipboard _clipboard;

    public virtual async Task CopyToClipboard()
    {
        StringBuilder stringBuilder = new StringBuilder();
        double timeInMinutes = 0;
        foreach (var item in FlightPlan.Timeline)
        {
            if (item is WaypointDetailsViewModel waypointDetails)
            {
                stringBuilder.AppendLine($"Waypoint");
                stringBuilder.AppendLine($"--------");
                stringBuilder.AppendLine($"Coordinates:      {waypointDetails.Coordinate.Latitude}; {waypointDetails.Coordinate.Longitude}");
                if (!string.IsNullOrWhiteSpace(waypointDetails.Coordinate.DisplayName))
                {
                    stringBuilder.AppendLine($"Coordinates:      {waypointDetails.Coordinate.DisplayName}");
                }
                if (waypointDetails.Coordinate.Elevation != null)
                {
                    stringBuilder.AppendLine($"Elevation:        {waypointDetails.Coordinate.Elevation} meters");
                }
                if (waypointDetails.Coordinate.City != null)
                {
                    stringBuilder.AppendLine($"City:             {waypointDetails.Coordinate.City}");
                }
                if (timeInMinutes > 0)
                {
                    stringBuilder.AppendLine($"Time to arrive here: {timeInMinutes} minutes");
                }
                stringBuilder.AppendLine();
            }
            if (item is FlightRouteLeg flightRouteLeg)
            {
                stringBuilder.AppendLine($"Route leg");
                stringBuilder.AppendLine($"---------");
                stringBuilder.AppendLine($"Rhumb bearing:    {flightRouteLeg.RhumbBearing}");
                stringBuilder.AppendLine($"Rhumb distance:   {flightRouteLeg.RhumbDistance} Km");
                stringBuilder.AppendLine($"Fuel consumption: {flightRouteLeg.FuelConsumption} l");
                stringBuilder.AppendLine($"Duration:         {flightRouteLeg.Time} minutes");
                stringBuilder.AppendLine();

                timeInMinutes += flightRouteLeg.Time;
            }
        }
        await _clipboard.SetTextAsync(stringBuilder.ToString());
    }
}
