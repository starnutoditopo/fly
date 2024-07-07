using Avalonia.Preferences;
using Fly.Models;
using Fly.Models.UnitsOfMeasure;
using System.Linq;

namespace Fly.Services;

public class SettingsService : ISettingsService
{
    private const string OPENAIP_API_BASE_URL = "OpenAIP_Api_BaseUrl";
    private const string OPENAIP_APIKEY = "OpenAIP_ApiKey";
    private const string OPENAIP_URLFORMATTER = "OpenAIP_Urlformatter";
    private const string OPENAIP_CSVSERVERSLIST = "OpenAIP_CsvServersList";
    private const string OPENAIP_USERAGENT = "OpenAIP_UserAgent";
    private const string DEFAULT_OPENAIP_API_BASE_URL = "https://api.core.openaip.net/api/";
    private const string DEFAULT_OPENAIP_URLFORMATTER = "https://{s}.api.tiles.openaip.net/api/data/openaip/{z}/{x}/{y}.png?apiKey={k}";
    private const string DEFAULT_OPENAIP_CSVSERVERSLIST = "a;b;c";
    private const string DEFAULT_OPENAIP_USERAGENT = "fly";

    private const string OPENSTREETMAP_URLFORMATTER = "OpenStreetMap_Urlformatter";
    private const string OPENSTREETMAP_USERAGENT = "OpenStreetMap_UserAgent";
    private const string DEFAULT_OPENSTREETMAP_URLFORMATTER = "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
    private const string DEFAULT_OPENSTREETMAP_USERAGENT = "fly";

    private const string MAPTILERSATELLITE_APIKEY = "MapTilerSatellite_ApiKey";
    private const string MAPTILERSATELLITE_URLFORMATTER = "MapTilerSatellite_Urlformatter";
    private const string MAPTILERSATELLITE_USERAGENT = "MapTilerSatellite_UserAgent";
    private const string DEFAULT_MAPTILERSATELLITE_URLFORMATTER = "https://api.maptiler.com/maps/hybrid/{z}/{x}/{y}.jpg?key={k}";
    private const string DEFAULT_MAPTILERSATELLITE_USERAGENT = "fly";

    private const string GENERAL_HOME_LONGITUDE = "Home_Longitude";
    private const string GENERAL_HOME_LATITUDE = "Home_Latitude";
    private const string GENERAL_AUTOMATICALLY_OPEN_LAST_DOCUMENT = "AutomaticallyOpenLastDocument";
    private const string GENERAL_LAST_OPENED_DOCUMENT = "LastOpenedDocument";

    private const string FLIGHT_RESIDUALAUTONOMY = "FLIGHT_RESIDUALAUTONOMY";
    private const string FLIGHT_TIMETOREACHALTERNATEFIELD = "Flight_TimeToReachAlternateField";

    private const string UOM_FAVOURITE_UNIT_OF_MEASURE_FOR_SPEED = "Uom_FavouriteUnitOfMeasureForSpeed";
    private const string UOM_FAVOURITE_UNIT_OF_MEASURE_FOR_FUEL_CONSUMPTION = "Uom_FavouriteUnitOfMeasureForFuelConsumption";
    //private const string NOMINATIM_BASEURL = "Nominatim_BaseUrl";
    //private const string NOMINATIM_USERAGENT = "Nominatim_UserAgent";
    //private const string NOMINATIM_REVERSEGEOCODEDETAILLEVEL = "Nominatim_ReverseGeocodeDetailLevel";
    private const string OPENELEVATION_USERAGENT = "OpenElevation_UserAgent";
    private const string OPENELEVATION_BASEURL = "OpenElevation_BaseUrl";

    private const string OPENROUTESERVICE_API_BASEURL = "OpenRouteService_Api_BaseUrl";
    private const string OPENROUTESERVICE_APIKEY = "GetOpenRouteService_ApiKey";
    private const string DEFAULT_OPENROUTESERVICE_API_BASEURL = "https://api.openrouteservice.org";

    private const string GENERAL_AUTOMATICALLY_LOAD_COORDINATE_INFORMATION = "AutomaticallyLoadCoordinateInformation";

    private readonly IUnitOfMeasureService _unitOfMeasureService;
    private readonly Preferences _preferences;
    public SettingsService(IUnitOfMeasureService unitOfMeasureService, Preferences preferences)
    {
        _unitOfMeasureService = unitOfMeasureService;
        _preferences = preferences;
    }

    public IUnitOfMeasure<Speed> GetFavouriteUnitOfMeasureForSpeed()
    {
        var fromSettings = _preferences.Get(UOM_FAVOURITE_UNIT_OF_MEASURE_FOR_SPEED, UnitsOfMeasure.KilometerPerHour.Code);
        var result = _unitOfMeasureService
            .GetAvailableUnitsOfMeasureForSpeed()
            .Single(r => r.Code == fromSettings);
        return result;
    }

    public void SetFavouriteUnitOfMeasureForSpeed(IUnitOfMeasure<Speed> unitOfMeasure)
    {
        _preferences.Set(UOM_FAVOURITE_UNIT_OF_MEASURE_FOR_SPEED, unitOfMeasure.Code);
    }

    public IUnitOfMeasure<FuelConsumption> GetFavouriteUnitOfMeasureForFuelConsumption()
    {
        var fromSettings = _preferences.Get(UOM_FAVOURITE_UNIT_OF_MEASURE_FOR_FUEL_CONSUMPTION, UnitsOfMeasure.LitersPerHour.Code);
        var result = _unitOfMeasureService
            .GetAvailableUnitsOfMeasureForFuelConsumption()
            .Single(r => r.Code == fromSettings);
        return result;
    }

    public void SetFavouriteUnitOfMeasureForFuelConsumption(IUnitOfMeasure<FuelConsumption> unitOfMeasure)
    {
        _preferences.Set(UOM_FAVOURITE_UNIT_OF_MEASURE_FOR_FUEL_CONSUMPTION, unitOfMeasure.Code);
    }


    /// <summary>
    /// Gets the time to reach alternate field (in minutes).
    /// </summary>
    /// <returns></returns>
    public int GetDefaultTimeToReachAlternateField()
    {
        var result = _preferences.Get(FLIGHT_TIMETOREACHALTERNATEFIELD, 30);
        return result;
    }

    /// <summary>
    /// Gets the residual autonomy (in minutes).
    /// </summary>
    public int GetDefaultResidualAutonomy()
    {
        var result = _preferences.Get(FLIGHT_RESIDUALAUTONOMY, 30);
        return result;
    }

    public CoordinateModel GetHome()
    {
        CoordinateModel result = new CoordinateModel()
        {
            Latitude = _preferences.Get(GENERAL_HOME_LATITUDE, 0.0),
            Longitude = _preferences.Get(GENERAL_HOME_LONGITUDE, 0.0)
        };
        return result;
    }

    public bool GetAutomaticallyOpenLastDocument()
    {
        var result = _preferences.Get(GENERAL_AUTOMATICALLY_OPEN_LAST_DOCUMENT, true);
        return result;
    }
    public void SetAutomaticallyOpenLastDocument(bool value)
    {
        _preferences.Set(GENERAL_AUTOMATICALLY_OPEN_LAST_DOCUMENT, value);
    }

    /// <summary>
    /// Gets the bookmark to the last opened document.
    /// </summary>
    /// <returns></returns>
    public string? GetLastOpenedDocument()
    {
        var bookmark = _preferences.Get(GENERAL_LAST_OPENED_DOCUMENT, (string?)null);
        return bookmark;
    }
    public void SetLastOpenedDocument(string? bookmark)
    {
        _preferences.Set(GENERAL_LAST_OPENED_DOCUMENT, bookmark);
    }

    public void SetHome(CoordinateModel value)
    {
        _preferences.Set(GENERAL_HOME_LONGITUDE, value.Longitude);
        _preferences.Set(GENERAL_HOME_LATITUDE, value.Latitude);
    }

    public void SetDefaultResidualAutonomy(int value)
    {
        _preferences.Set(FLIGHT_RESIDUALAUTONOMY, value);
    }

    public void SetDefaultTimeToReachAlternateField(int value)
    {
        _preferences.Set(FLIGHT_TIMETOREACHALTERNATEFIELD, value);
    }

    #region Nominatim
    //public string GetNominatim_BaseUrl()
    //{
    //    var result = _preferences.Get(NOMINATIM_BASEURL, string.Empty);
    //    return result;
    //}
    //public void SetNominatim_BaseUrl(string value)
    //{
    //    _preferences.Set(NOMINATIM_BASEURL, value);
    //}

    //public ReverseGeocodeDetailLevel GetNominatim_ReverseGeocodeDetailLevel()
    //{
    //    var result = (ReverseGeocodeDetailLevel)_preferences.Get(NOMINATIM_REVERSEGEOCODEDETAILLEVEL, (int)ReverseGeocodeDetailLevel.Building);
    //    return result;
    //}
    //public void SetNominatim_ReverseGeocodeDetailLevel(ReverseGeocodeDetailLevel value)
    //{
    //    _preferences.Set(NOMINATIM_REVERSEGEOCODEDETAILLEVEL, (int)value);
    //}
    //public string GetNominatim_UserAgent()
    //{
    //    var result = _preferences.Get(NOMINATIM_USERAGENT, string.Empty);
    //    return result;
    //}
    //public void SetNominatim_UserAgent(string value)
    //{
    //    _preferences.Set(NOMINATIM_USERAGENT, value);
    //}
    #endregion

    #region OpenElevation
    //public string GetOpenElevation_BaseUrl()
    //{
    //    var result = _preferences.Get(OPENELEVATION_BASEURL, DEFAULT_OPENELEVATION_BASEURL);
    //    return result;
    //}
    //public void SetOpenElevation_BaseUrl(string value)
    //{
    //    _preferences.Set(OPENELEVATION_BASEURL, value);
    //}
    //public string GetOpenElevation_UserAgent()
    //{
    //    var result = _preferences.Get(OPENELEVATION_USERAGENT, DEFAULT_OPENELEVATION_USERAGENT);
    //    return result;
    //}
    //public void SetOpenElevation_UserAgent(string value)
    //{
    //    _preferences.Set(OPENELEVATION_USERAGENT, value);
    //}
    #endregion

    #region OpenAIP
    public string GetOpenAIP_UserAgent()
    {
        var result = _preferences.Get(OPENAIP_USERAGENT, DEFAULT_OPENAIP_USERAGENT);
        return result;
    }
    public string[] GetOpenAIP_Map_ServersList()
    {
        var csvServersList = _preferences.Get(OPENAIP_CSVSERVERSLIST, DEFAULT_OPENAIP_CSVSERVERSLIST);
        var result = StringArraySerializationHelper.FromCsv(csvServersList)
            .Select(i => i.Trim())
            .Distinct()
            .Where(i => i.Length > 0)
            .ToArray();
        return result;
    }
    public void SetOpenAIP_Map_ServersList(string[] value)
    {
        if (value == null)
        {
            value = [];
        }
        value = value
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .Distinct()
            .ToArray();
        string csvServersList = StringArraySerializationHelper.ToCsv(value);

        _preferences.Set(OPENAIP_CSVSERVERSLIST, csvServersList);
    }
    public string GetOpenAIP_Map_Urlformatter()
    {
        var result = _preferences.Get(OPENAIP_URLFORMATTER, DEFAULT_OPENAIP_URLFORMATTER);
        return result;
    }
    public string GetOpenAIP_ApiKey()
    {
        var result = _preferences.Get(OPENAIP_APIKEY, string.Empty);
        return result;
    }
    public void SetOpenAIP_ApiKey(string value)
    {
        _preferences.Set(OPENAIP_APIKEY, value);
    }
    public void SetOpenAIP_Map_Urlformatter(string value)
    {
        _preferences.Set(OPENAIP_URLFORMATTER, value);
    }
    public void SetOpenAIP_UserAgent(string value)
    {
        _preferences.Set(OPENAIP_USERAGENT, value);
    }
    public string GetOpenAIP_Api_BaseUrl()
    {
        var result = _preferences.Get(OPENAIP_API_BASE_URL, DEFAULT_OPENAIP_API_BASE_URL);
        return result;
    }

    public void SetOpenAIP_Api_BaseUrl(string value)
    {
        _preferences.Set(OPENAIP_API_BASE_URL, value);
    }
    #endregion

    #region OpenRouteService - elevation
    public string GetOpenRouteService_Api_BaseUrl()
    {
        var result = _preferences.Get(OPENROUTESERVICE_API_BASEURL, DEFAULT_OPENROUTESERVICE_API_BASEURL);
        return result;
    }
    public void SetOpenRouteService_Api_BaseUrl(string value)
    {
        _preferences.Set(OPENROUTESERVICE_API_BASEURL, value);
    }
    public string GetOpenRouteService_ApiKey()
    {
        var result = _preferences.Get(OPENROUTESERVICE_APIKEY, string.Empty);
        return result;
    }
    public void SetOpenRouteService_ApiKey(string value)
    {
        _preferences.Set(OPENROUTESERVICE_APIKEY, value);
    }
    #endregion

    public bool GetAutomaticallyLoadCoordinateInformation()
    {
        var result = _preferences.Get(GENERAL_AUTOMATICALLY_LOAD_COORDINATE_INFORMATION, true);
        return result;
    }

    public void SetAutomaticallyLoadCoordinateInformation(bool value)
    {
        _preferences.Set(GENERAL_AUTOMATICALLY_LOAD_COORDINATE_INFORMATION, value);
    }

    #region MapTiler - Satellite
    public string GetMapTilerSatellite_UserAgent()
    {
        var result = _preferences.Get(MAPTILERSATELLITE_USERAGENT, DEFAULT_MAPTILERSATELLITE_USERAGENT);
        return result;
    }
    public string GetMapTilerSatellite_Map_Urlformatter()
    {
        var result = _preferences.Get(MAPTILERSATELLITE_URLFORMATTER, DEFAULT_MAPTILERSATELLITE_URLFORMATTER);
        return result;
    }
    public string GetMapTilerSatellite_ApiKey()
    {
        var result = _preferences.Get(MAPTILERSATELLITE_APIKEY, string.Empty);
        return result;
    }
    public void SetMapTilerSatellite_ApiKey(string value)
    {
        _preferences.Set(MAPTILERSATELLITE_APIKEY, value);
    }
    public void SetMapTilerSatellite_Map_Urlformatter(string value)
    {
        _preferences.Set(MAPTILERSATELLITE_URLFORMATTER, value);
    }
    public void SetMapTilerSatellite_UserAgent(string value)
    {
        _preferences.Set(MAPTILERSATELLITE_USERAGENT, value);
    }
    #endregion

    #region OpenStreetMap
    public string GetOpenStreetMap_UserAgent()
    {
        var result = _preferences.Get(OPENSTREETMAP_USERAGENT, DEFAULT_OPENSTREETMAP_USERAGENT);
        return result;
    }
    public string GetOpenStreetMap_Map_Urlformatter()
    {
        var result = _preferences.Get(OPENSTREETMAP_URLFORMATTER, DEFAULT_OPENSTREETMAP_URLFORMATTER);
        return result;
    }
    public void SetOpenStreetMap_Map_Urlformatter(string value)
    {
        _preferences.Set(OPENSTREETMAP_URLFORMATTER, value);
    }
    public void SetOpenStreetMap_UserAgent(string value)
    {
        _preferences.Set(OPENSTREETMAP_USERAGENT, value);
    }
    #endregion

}