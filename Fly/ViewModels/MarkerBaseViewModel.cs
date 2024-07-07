using Fly.Services;

namespace Fly.ViewModels;

public abstract class MarkerBaseViewModel : ViewModelBase
{
    protected MarkerBaseViewModel(
        ISettingsService settingsService,
        IReverseGeocodeService reverseGeocodeService,
        IElevationService elevationService,
        IAirspaceInformationService airspaceInformationService
        )
    {
        Coordinate = new CoordinateViewModel(settingsService, elevationService, reverseGeocodeService, airspaceInformationService);
        Coordinate.DisplayName = Constants.MARKER_DEFAULT_NAME;
        _isVisible = true;
    }

    public virtual CoordinateBaseViewModel Coordinate { get; }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }
}
