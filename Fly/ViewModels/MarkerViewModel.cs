// using Geo;
using Fly.Models;
using Fly.Services;

namespace Fly.ViewModels;

public class MarkerViewModel : MarkerBaseViewModel
{
    /// <summary>
    /// Utility ctor that initializes a new instance of the <see cref="MarkerViewModel"/> class, setting its coordinate's values.
    /// </summary>
    /// <param name="coordinate">The coordinate.</param>
    public MarkerViewModel(
        ISettingsService settingsService,
        IReverseGeocodeService reverseGeocodeService,
        IElevationService elevationService,
        IAirspaceInformationService airspaceInformationService
        )
        : base(settingsService, reverseGeocodeService, elevationService, airspaceInformationService)
    {
    }

    public virtual CoordinateModel? CoordinateAtDraggingStart { get; set; }
    public virtual CoordinateModel? MouseStartDraggingCoordinate { get; set; }
}
