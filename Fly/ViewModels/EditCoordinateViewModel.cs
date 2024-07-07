using AutoMapper;
using Fly.Models;
using Fly.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Fly.ViewModels;

public class EditCoordinateViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    public EditCoordinateViewModel(
        IMapper mapper,
        IReverseGeocodeService reverseGeocodeService,
        IElevationService elevationService,
        ISettingsService settingsService,
        IAirspaceInformationService airspaceInformationService,
        FullCoordinateInformationModel fullCoordinateInformationModel
    )
    {
        _settingsService = settingsService;

        _coordinateViewModel  = mapper.Map<CoordinateViewModel>(fullCoordinateInformationModel);
    }

    private CoordinateViewModel _coordinateViewModel;
    public CoordinateViewModel CoordinateViewModel
    {
        get => _coordinateViewModel;
        private set => SetProperty(ref _coordinateViewModel, value);
    }


    public async Task Loaded()
    {
        if (_settingsService.GetAutomaticallyLoadCoordinateInformation())
        {
            if (this.CoordinateViewModel.Elevation == null)
            {
                await this.CoordinateViewModel.UpdateElevationInformation();
            }
            if (
                string.IsNullOrWhiteSpace(this.CoordinateViewModel.City) &&
                string.IsNullOrWhiteSpace(this.CoordinateViewModel.DisplayName)
                )
            {
                await this.CoordinateViewModel.UpdateGeoCodingInformation();
            }
            if (!this.CoordinateViewModel.AirspaceInformationItems.Any())
            {
                await this.CoordinateViewModel.UpdateAirspaceInformation();
            }
        }
    }
}
