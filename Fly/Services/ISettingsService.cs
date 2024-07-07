using Fly.Models;
using Fly.Models.UnitsOfMeasure;

namespace Fly.Services;

public interface ISettingsService
{
    #region Plane
    IUnitOfMeasure<Speed> GetFavouriteUnitOfMeasureForSpeed();
    void SetFavouriteUnitOfMeasureForSpeed(IUnitOfMeasure<Speed> unitOfMeasure);
    IUnitOfMeasure<FuelConsumption> GetFavouriteUnitOfMeasureForFuelConsumption();
    void SetFavouriteUnitOfMeasureForFuelConsumption(IUnitOfMeasure<FuelConsumption> unitOfMeasure);
    #endregion

    #region General
    CoordinateModel GetHome();
    void SetHome(CoordinateModel value);

    bool GetAutomaticallyOpenLastDocument();
    void SetAutomaticallyOpenLastDocument(bool value);

    string? GetLastOpenedDocument();
    void SetLastOpenedDocument(string? bookmark);

    bool GetAutomaticallyLoadCoordinateInformation();
    void SetAutomaticallyLoadCoordinateInformation(bool value);
    #endregion

    #region Open AIP
    string GetOpenAIP_UserAgent();
    string[] GetOpenAIP_Map_ServersList();
    string GetOpenAIP_Map_Urlformatter();
    string GetOpenAIP_ApiKey();
    string GetOpenAIP_Api_BaseUrl();
    void SetOpenAIP_ApiKey(string value);
    void SetOpenAIP_Map_Urlformatter(string value);
    void SetOpenAIP_Map_ServersList(string[] value);
    void SetOpenAIP_UserAgent(string value);
    void SetOpenAIP_Api_BaseUrl(string value);
    #endregion

    #region Flight
    int GetDefaultTimeToReachAlternateField();
    void SetDefaultTimeToReachAlternateField(int value);
    int GetDefaultResidualAutonomy();
    void SetDefaultResidualAutonomy(int value);
    #endregion

    #region shell
    //double GetShellWindowWidth();
    //void SetShellWindowWidth(double value);
    //WindowState GetShellWindowState();
    //void SetShellWindowState(WindowState value);
    //double GetShellWindowTop();
    //void SetShellWindowTop(double value);
    //double GetShellWindowLeft();
    //void SetShellWindowLeft(double value);
    //double GetShellWindowHeight();
    //void SetShellWindowHeight(double value);
    #endregion

    #region Nominatim
    //string GetNominatim_BaseUrl();
    //void SetNominatim_BaseUrl(string value);
    //ReverseGeocodeDetailLevel GetNominatim_ReverseGeocodeDetailLevel();
    //void SetNominatim_ReverseGeocodeDetailLevel(ReverseGeocodeDetailLevel value);
    //string GetNominatim_UserAgent();
    //void SetNominatim_UserAgent(string value);
    #endregion

    #region Open elevation
    //string GetOpenElevation_BaseUrl();
    //void SetOpenElevation_BaseUrl(string value);
    //string GetOpenElevation_UserAgent();
    //void SetOpenElevation_UserAgent(string value);
    #endregion

    #region OpenRouteService - elevation
    string GetOpenRouteService_Api_BaseUrl();
    void SetOpenRouteService_Api_BaseUrl(string value);
    string GetOpenRouteService_ApiKey();
    void SetOpenRouteService_ApiKey(string value);
    #endregion

    #region MapTiler - Satellite
    string GetMapTilerSatellite_UserAgent();
    string GetMapTilerSatellite_Map_Urlformatter();
    string GetMapTilerSatellite_ApiKey();
    void SetMapTilerSatellite_ApiKey(string value);
    void SetMapTilerSatellite_Map_Urlformatter(string value);
    void SetMapTilerSatellite_UserAgent(string value);
    #endregion

    #region OpenStreetMap
    string GetOpenStreetMap_UserAgent();
    string GetOpenStreetMap_Map_Urlformatter();
    void SetOpenStreetMap_Map_Urlformatter(string value);
    void SetOpenStreetMap_UserAgent(string value);
    #endregion
}
