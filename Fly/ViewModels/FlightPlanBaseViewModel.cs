using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Fly.Extensions;
using Fly.Models;
using Fly.Models.UnitsOfMeasure;

namespace Fly.ViewModels;

public abstract class FlightPlanBaseViewModel : ViewModelBase
{
    private static double ConvertQuantity<TQuantity>(
        double quantity,
        IUnitOfMeasure<TQuantity> originUnitOfMeasure,
        IUnitOfMeasure<TQuantity> targetUnitOfMeasure
        )
        where TQuantity : IQuantity
    {
        if (originUnitOfMeasure == targetUnitOfMeasure)
        {
            return quantity;
        }
        #region kt and Km/h
        if (originUnitOfMeasure == UnitsOfMeasure.KilometerPerHour)
        {
            if (targetUnitOfMeasure == UnitsOfMeasure.Knot)
            {
                return quantity / 1.852;
            }
        }
        if (originUnitOfMeasure == UnitsOfMeasure.Knot)
        {
            if (targetUnitOfMeasure == UnitsOfMeasure.KilometerPerHour)
            {
                return quantity * 1.852;
            }
        }
        #endregion

        throw new NotImplementedException();
    }
    protected FlightPlanBaseViewModel()
    {
        _displayName = "New flight plan";
        Timeline = new ObservableCollection<ITimelineItem>();
        PropertyChanged += FlightPlanBaseViewModel_PropertyChanged;
    }

    private void FlightPlanBaseViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if ((e.PropertyName == nameof(Plane))
            || (e.PropertyName == nameof(Route)))
        {
            RecalculateTimeline();
        }
    }

    private void RecalculateTimeline()
    {
        Timeline.Clear();
        if (Plane != null)
        {
            if (Route != null)
            {
                double meanFuelConsumptionInLitersPerHour = ConvertQuantity(Plane.MeanFuelConsumption, Plane.MeanFuelConsumptionUnitOfMeasure, UnitsOfMeasure.LitersPerHour);
                double cruiseSpeedInKmPerHour = ConvertQuantity(Plane.CruiseSpeed, Plane.CruiseSpeedUnitOfMeasure, UnitsOfMeasure.KilometerPerHour);
                IEnumerable<ITimelineItem> items = GetTimelineItems(
                    meanFuelConsumptionInLitersPerHour,
                    Route.Coordinates,
                    cruiseSpeedInKmPerHour
                );
                foreach (var item in items)
                {
                    Timeline.Add(item);
                }
                FlightDuration = TimeSpan.FromHours(Route.RouteLength / cruiseSpeedInKmPerHour);
                TotalRequiredFuel = Math.Ceiling( items
                    .OfType<FlightRouteLeg>()
                    .Sum(i => i.FuelConsumption)
                    + GetFuelConsumption(meanFuelConsumptionInLitersPerHour, ResidualAutonomy)
                    + GetFuelConsumption(meanFuelConsumptionInLitersPerHour, TimeToReachAlternateField));

                return;
            }
        }

        FlightDuration = null;
        TotalRequiredFuel = null;
    }


    /// <summary>
    /// Gets the fuel consumption, in liters.
    /// </summary>
    /// <param name="meanFuelConsumption">The mean fuel consumption, in liters per hour (l/h).</param>
    /// <param name="time">The time, in minutes.</param>
    /// <returns>The fuel consumption, in liters.</returns>
    private static double GetFuelConsumption(
        double meanFuelConsumption,
        double time)
    {
        var fuelConsumption = time * meanFuelConsumption / 60;
        return fuelConsumption;
    }

    private static IEnumerable<ITimelineItem> GetTimelineItems(
        double meanFuelConsumptionInLitersPerHour,
        IList<CoordinateBaseViewModel>? waypointCoordinates,
        double cruiseSpeedInKmPerHour
    )
    {
        if (waypointCoordinates != null)
        {
            for (int i = 0; i < waypointCoordinates.Count - 1; i++)
            {
                var from = waypointCoordinates[i];
                WaypointDetailsViewModel waypointDetails = new WaypointDetailsViewModel(from);
                yield return waypointDetails;


                var to = waypointCoordinates[i + 1];

                // the rhumb distance, in Km
                var rhumbDistance = GeographyExtensions.GetRhumbDistance(from.Longitude, from.Latitude, to.Longitude, to.Latitude) / 1000;

                // the time, in minutes
                var time = rhumbDistance / cruiseSpeedInKmPerHour * 60;

                // the fuel consumption, in liters (l)
                var fuelConsumption = GetFuelConsumption(meanFuelConsumptionInLitersPerHour, time);

                FlightRouteLeg flightRouteLeg = new FlightRouteLeg()
                {
                    RhumbBearing = GeographyExtensions.GetRhumbBearing(from.Longitude, from.Latitude, to.Longitude, to.Latitude),
                    RhumbDistance = rhumbDistance,
                    Distance = GeographyExtensions.GetDistance(from.Longitude, from.Latitude, to.Longitude, to.Latitude) / 1000,
                    FuelConsumption = fuelConsumption,
                    Time = time
                };
                FlightRouteLegViewModel flightRouteLegViewModel = new FlightRouteLegViewModel(flightRouteLeg);
                yield return flightRouteLegViewModel;
            }
            yield return new WaypointDetailsViewModel(waypointCoordinates.Last());
        }
    }


    private string _displayName;
    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    private int _residualAutonomy;
    /// <summary>
    /// Gets or sets the residual autonomy, in minutes.
    /// </summary>
    /// <value>
    /// The residual autonomy, in minutes.
    /// </value>
    public int ResidualAutonomy
    {
        get => _residualAutonomy;
        set => SetProperty(ref _residualAutonomy, value);
    }

    private int _timeToReachAlternateField;
    /// <summary>
    /// Gets or sets the time to reach alternate field, in minutes.
    /// </summary>
    /// <value>
    /// The time to reach alternate field, in minutes.
    /// </value>
    public int TimeToReachAlternateField
    {
        get => _timeToReachAlternateField;
        set => SetProperty(ref _timeToReachAlternateField, value);
    }

    private RouteBaseViewModel? _route;
    public RouteBaseViewModel? Route
    {
        get => _route;
        set => SetProperty(ref _route, value);
    }

    private PlaneBaseViewModel? _plane;
    public PlaneBaseViewModel? Plane
    {
        get => _plane;
        set => SetProperty(ref _plane, value);
    }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private TimeSpan? _flightDuration;
    public TimeSpan? FlightDuration
    {
        get=> _flightDuration;
        set => SetProperty(ref _flightDuration, value);
    }

    public ObservableCollection<ITimelineItem> Timeline { get; }

    private double? _totalRequiredFuel;
    public double? TotalRequiredFuel
    {
        get => _totalRequiredFuel;
        set => SetProperty(ref _totalRequiredFuel, value);
    }
}
