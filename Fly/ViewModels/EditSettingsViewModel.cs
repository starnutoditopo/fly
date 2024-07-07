using Fly.Models;
using Fly.Services;
using Fly.Models.UnitsOfMeasure;
using System.Collections.Generic;

namespace Fly.ViewModels;

public class EditSettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IEqualityComparer<CoordinateModel> _coordinateModelEqualityComparer;
    public EditSettingsViewModel(
        IUnitOfMeasureService unitOfMeasureService,
        IEqualityComparer<CoordinateModel> coordinateModelEqualityComparer,
        ISettingsService settingsService
    )
    {
        _coordinateModelEqualityComparer = coordinateModelEqualityComparer;
        _settingsService = settingsService;
        AvailableUnitsOfMeasureForSpeed = unitOfMeasureService.GetAvailableUnitsOfMeasureForSpeed();
        AvailableUnitsOfMeasureForFuelConsumption = unitOfMeasureService.GetAvailableUnitsOfMeasureForFuelConsumption();
    }

    #region OpenAIP
    public string OpenAIP_ApiKey
    {
        get => _settingsService.GetOpenAIP_ApiKey();
        set => SetProperty(_settingsService.GetOpenAIP_ApiKey(), value, _settingsService.SetOpenAIP_ApiKey);
    }

    public string OpenAIP_Urlformatter
    {
        get => _settingsService.GetOpenAIP_Map_Urlformatter();
        set => SetProperty(_settingsService.GetOpenAIP_Map_Urlformatter(), value, _settingsService.SetOpenAIP_Map_Urlformatter);
    }

    public string[] OpenAIP_ServersList
    {
        get => _settingsService.GetOpenAIP_Map_ServersList();
        set => SetProperty(_settingsService.GetOpenAIP_Map_ServersList(), value, _settingsService.SetOpenAIP_Map_ServersList);

    }

    public string OpenAIP_UserAgent
    {
        get => _settingsService.GetOpenAIP_UserAgent();
        set => SetProperty(_settingsService.GetOpenAIP_UserAgent(), value, _settingsService.SetOpenAIP_UserAgent);
    }
    #endregion

    public CoordinateModel Home
    {
        get => _settingsService.GetHome();
        set => SetProperty(_settingsService.GetHome(), value, _coordinateModelEqualityComparer, _settingsService.SetHome);
    }

    public int Flight_ResidualAutonomy
    {
        get => _settingsService.GetDefaultResidualAutonomy();
        set => SetProperty(_settingsService.GetDefaultResidualAutonomy(), value, _settingsService.SetDefaultResidualAutonomy);
    }

    public int Flight_TimeToReachAlternateField
    {
        get => _settingsService.GetDefaultTimeToReachAlternateField();
        set => SetProperty(_settingsService.GetDefaultTimeToReachAlternateField(), value, _settingsService.SetDefaultTimeToReachAlternateField);
    }

    public IUnitOfMeasure<Speed> Plane_FavouriteUnitOfMeasureForSpeed
    {
        get => _settingsService.GetFavouriteUnitOfMeasureForSpeed();
        set => SetProperty(_settingsService.GetFavouriteUnitOfMeasureForSpeed(), value, _settingsService.SetFavouriteUnitOfMeasureForSpeed);
    }
    public IUnitOfMeasure<FuelConsumption> Plane_FavouriteUnitOfMeasureForFuelConsumption
    {
        get => _settingsService.GetFavouriteUnitOfMeasureForFuelConsumption();
        set => SetProperty(_settingsService.GetFavouriteUnitOfMeasureForFuelConsumption(), value, _settingsService.SetFavouriteUnitOfMeasureForFuelConsumption);
    }

    public IEnumerable<IUnitOfMeasure<Speed>> AvailableUnitsOfMeasureForSpeed { get; }
    public IEnumerable<IUnitOfMeasure<FuelConsumption>> AvailableUnitsOfMeasureForFuelConsumption { get; }

    public bool AutomaticallyOpenLastDocument
    {
        get => _settingsService.GetAutomaticallyOpenLastDocument();
        set => SetProperty(_settingsService.GetAutomaticallyOpenLastDocument(), value, _settingsService.SetAutomaticallyOpenLastDocument);
    }

    public string OpenAIP_Api_BaseUrl
    {
        get => _settingsService.GetOpenAIP_Api_BaseUrl();
        set => SetProperty(_settingsService.GetOpenAIP_Api_BaseUrl(), value, _settingsService.SetOpenAIP_Api_BaseUrl);
    }

    public string OpenRouteService_Api_BaseUrl
    {
        get => _settingsService.GetOpenRouteService_Api_BaseUrl();
        set => SetProperty(_settingsService.GetOpenRouteService_Api_BaseUrl(), value, _settingsService.SetOpenRouteService_Api_BaseUrl);
    }

    public string OpenRouteService_ApiKey
    {
        get => _settingsService.GetOpenRouteService_ApiKey();
        set => SetProperty(_settingsService.GetOpenRouteService_ApiKey(), value, _settingsService.SetOpenRouteService_ApiKey);
    }
    public bool AutomaticallyLoadCoordinateInformation
    {
        get => _settingsService.GetAutomaticallyLoadCoordinateInformation();
        set => SetProperty(_settingsService.GetAutomaticallyLoadCoordinateInformation(), value, _settingsService.SetAutomaticallyLoadCoordinateInformation);
    }

    #region Maptiler satellite
    public string MapTilerSatellite_ApiKey
    {
        get => _settingsService.GetMapTilerSatellite_ApiKey();
        set => SetProperty(_settingsService.GetMapTilerSatellite_ApiKey(), value, _settingsService.SetMapTilerSatellite_ApiKey);
    }

    public string MapTilerSatellite_Urlformatter
    {
        get => _settingsService.GetMapTilerSatellite_Map_Urlformatter();
        set => SetProperty(_settingsService.GetMapTilerSatellite_Map_Urlformatter(), value, _settingsService.SetMapTilerSatellite_Map_Urlformatter);
    }

    public string MapTilerSatellite_UserAgent
    {
        get => _settingsService.GetMapTilerSatellite_UserAgent();
        set => SetProperty(_settingsService.GetMapTilerSatellite_UserAgent(), value, _settingsService.SetMapTilerSatellite_UserAgent);
    }
    #endregion

    #region OpenStreetMap
    public string OpenStreetMap_Urlformatter
    {
        get => _settingsService.GetOpenStreetMap_Map_Urlformatter();
        set => SetProperty(_settingsService.GetOpenStreetMap_Map_Urlformatter(), value, _settingsService.SetOpenStreetMap_Map_Urlformatter);
    }

    public string OpenStreetMap_UserAgent
    {
        get => _settingsService.GetOpenStreetMap_UserAgent();
        set => SetProperty(_settingsService.GetOpenStreetMap_UserAgent(), value, _settingsService.SetOpenStreetMap_UserAgent);
    }
    #endregion
}
