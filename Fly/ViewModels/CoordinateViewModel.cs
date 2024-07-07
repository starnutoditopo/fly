using Fly.Models;
using Fly.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Fly.ViewModels;

public class CoordinateViewModel : CoordinateBaseViewModel
{
    private readonly IReverseGeocodeService _reverseGeocodeService;
    private readonly IElevationService _elevationService;
    private readonly IAirspaceInformationService _airspaceInformationService;
    public CoordinateViewModel(
        ISettingsService settingsService,
        IElevationService elevationService,
        IReverseGeocodeService reverseGeocodeService,
        IAirspaceInformationService airspaceInformationService
    )
    {
        _reverseGeocodeService = reverseGeocodeService;
        _elevationService = elevationService;
        _airspaceInformationService = airspaceInformationService;
        SettingsService = settingsService;
        AirspaceInformationItems = new ObservableCollection<AirspacesInformationItemViewModel>();
    }
    public ISettingsService SettingsService { get; }

    public ObservableCollection<AirspacesInformationItemViewModel> AirspaceInformationItems { get; }

    public async Task UpdateGeoCodingInformation()
    {
        CoordinateModel coordinateModel = new CoordinateModel()
        {
            Latitude = Latitude,
            Longitude = Longitude
        };
        var result = await _reverseGeocodeService.GetGeocodeInformationForCoordinates([coordinateModel]);
        var firstResult = result[0];
        if (!string.IsNullOrWhiteSpace(firstResult.DisplayName))
        {
            DisplayName = firstResult.DisplayName;
        }
        if (!string.IsNullOrWhiteSpace(firstResult.City))
        {
            City = firstResult.City;
        }
    }

    public async Task UpdateElevationInformation()
    {
        CoordinateModel coordinateModel = new CoordinateModel()
        {
            Latitude = Latitude,
            Longitude = Longitude
        };
        var result = await _elevationService.GetElevationForCoordinates([coordinateModel]);
        var firstResult = result[0];
        if (firstResult != null)
        {
            Elevation = firstResult.Value;
        }
    }

    public async Task UpdateAirspaceInformation()
    {
        this.AirspaceInformationItems.Clear();
        CoordinateModel coordinateModel = new CoordinateModel()
        {
            Latitude = Latitude,
            Longitude = Longitude
        };
        var airspaceInformation = await _airspaceInformationService.GetAirspaceInformation(coordinateModel);
        if (airspaceInformation.Items != null)
        {
            foreach (var item in airspaceInformation.Items.Select(i => new AirspacesInformationItemViewModel(i)))
            {
                this.AirspaceInformationItems.Add(item);
            }
        }
    }
}
