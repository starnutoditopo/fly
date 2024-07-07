using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Fly.Models;
using Fly.Models.OpenAip;
using DynamicData;

namespace Fly.Services;
public class OpenAipService : IAirspaceInformationService
{
    private readonly HttpClient _httpClient;
    private readonly ISettingsService _settingsService;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="baseUrl">Template URL to OpenAip service.</param>
    public OpenAipService(
        HttpClient httpClient,
        ISettingsService settingsService
        )
    {
        _httpClient = httpClient;
        _settingsService = settingsService;
    }

    /// <summary>
    /// Translate the OpenAIP value to an Icao class.
    /// According to the OpenAIP schema ( https://docs.openaip.net/#/Airspaces/get_airspaces , select function, click on "schema"),
    /// The possible values for airspace ICAO classes are:
    /// - 0: A
    /// - 1: B
    /// - 2: C
    /// - 3: D
    /// - 4: E
    /// - 5: F
    /// - 6: G
    /// - 8: Unclassified / Special Use Airspace(SUA)
    /// </summary>
    /// <param name="l"></param>
    /// <returns></returns>
    private static IcaoClass ToIcaoClass(long l)
    {
        if (l >= 0)
        {
            if (l <= 6)
            {
                return (IcaoClass)l;
            }
        }
        if (l == 8)
        {
            return IcaoClass.Unclassified;
        }

        throw new NotSupportedException($"Unrecognized value - {l}.");
    }

    /// <summary>
    /// Translate the OpenAIP value to an Icao class.
    /// According to the OpenAIP schema ( https://docs.openaip.net/#/Airspaces/get_airspaces , select function, click on "schema"),
    /// The possible values for airspace types are:
    /// - 0: Other
    /// - 1: Restricted
    /// - 2: Danger
    /// - 3: Prohibited
    /// - 4: Controlled Tower Region (CTR)
    /// - 5: Transponder Mandatory Zone (TMZ)
    /// - 6: Radio Mandatory Zone (RMZ)
    /// - 7: Terminal Maneuvering Area (TMA)
    /// - 8: Temporary Reserved Area (TRA)
    /// - 9: Temporary Segregated Area (TSA)
    /// - 10: Flight Information Region (FIR)
    /// - 11: Upper Flight Information Region (UIR)
    /// - 12: Air Defense Identification Zone (ADIZ)
    /// - 13: Airport Traffic Zone (ATZ)
    /// - 14: Military Airport Traffic Zone (MATZ)
    /// - 15: Airway
    /// - 16: Military Training Route (MTR)
    /// - 17: Alert Area
    /// - 18: Warning Area
    /// - 19: Protected Area
    /// - 20: Helicopter Traffic Zone (HTZ)
    /// - 21: Gliding Sector
    /// - 22: Transponder Setting (TRP)
    /// - 23: Traffic Information Zone (TIZ)
    /// - 24: Traffic Information Area (TIA)
    /// - 25: Military Training Area (MTA)
    /// - 26: Control Area (CTA)
    /// - 27: ACC Sector (ACC)
    /// - 28: Aerial Sporting Or Recreational Activity
    /// - 29: Low Altitude Overflight Restriction
    /// - 30: Military Route (MRT)
    /// - 31: TSA/TRA Feeding Route (TFR)
    /// - 32: VFR Sector
    /// - 33: FIS Sector
    /// </summary>
    /// <param name="l"></param>
    /// <returns></returns>
    private static AirspaceType ToAirspaceTypeClass(long l)
    {
        var values = Enum
            .GetValues(typeof(AirspaceType))
            .Cast<int>()
            .ToHashSet();
        if (values.Contains((int)l))
        {
            return (AirspaceType)l;
        }
        throw new NotSupportedException($"Unrecognized value - {l}.");
    }

    private static VerticalLimitUnit ToVerticalUnit(long l)
    {
        if (l == 0)
        {
            return VerticalLimitUnit.Meter;
        }
        if (l == 1)
        {
            return VerticalLimitUnit.Feet;
        }
        if (l == 6)
        {
            return VerticalLimitUnit.FlightLevel;
        }

        throw new NotSupportedException($"Unrecognized value - {l}.");
    }

    private static VerticalLimitReferenceDatum ToReferenceDatum(long l)
    {
        if (l == 0)
        {
            return VerticalLimitReferenceDatum.GND;
        }
        if (l == 1)
        {
            return VerticalLimitReferenceDatum.MSL;
        }
        if (l == 2)
        {
            return VerticalLimitReferenceDatum.STD;
        }

        throw new NotSupportedException($"Unrecognized value - {l}.");
    }

    private static VerticalLimit ToLimit(ErLimit erLimit)
    {
        VerticalLimit result = new VerticalLimit()
        {
            Value = erLimit.Value,
            Unit = ToVerticalUnit(erLimit.Unit),
            ReferenceDatum = ToReferenceDatum(erLimit.ReferenceDatum)
        };
        return result;
    }

    private static AirspacesInformationItemModel ToAirspacesInformationItem(Item item)
    {
        AirspacesInformationItemModel resultItem = new AirspacesInformationItemModel()
        {
            Type = ToAirspaceTypeClass(item.Type),
            IcaoClass = ToIcaoClass(item.IcaoClass),
            Name = item.Name,
            Country = item.Country,
            Frequencies = item.Frequencies,
            Geometry = item.Geometry,
            LowerLimit = ToLimit(item.LowerLimit),
            UpperLimit = ToLimit(item.UpperLimit),
        };
        return resultItem;
    }

    public async Task<AirspacesInformationModel> GetAirspaceInformation(CoordinateModel coordinate, CancellationToken cancellationToken = default)
    {
        AirspacesInformationModel result = new AirspacesInformationModel();

        RawGetAirspacesResponse rawGetAirspacesResponse;
        #region get rawGetAirspacesResponse

        //var jsonString = await System.IO.File.ReadAllTextAsync("C:\\mpd\\WpfFly\\Fly\\Fly\\Fly\\Models\\OpenAip\\GetAirspacesResponse.Samples\\Airspaces2.json");
        //rawGetAirspacesResponse = RawGetAirspacesResponse.FromJson(jsonString);

        string apiKey = _settingsService.GetOpenAIP_ApiKey();
        string baseUrl = _settingsService.GetOpenAIP_Api_BaseUrl();
        string url = $"{baseUrl}airspaces?pos={coordinate.Latitude.ToString(CultureInfo.InvariantCulture)},{coordinate.Longitude.ToString(CultureInfo.InvariantCulture)}&dist=150&apiKey={apiKey}";
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
        {
            string userAgent = _settingsService.GetOpenAIP_UserAgent();
            httpRequest.Headers.Add("User-Agent", new string[] { userAgent });

            var response = await _httpClient.SendAsync(httpRequest);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw;
            }

            rawGetAirspacesResponse = await response.Content.ReadFromJsonAsync<RawGetAirspacesResponse>(Converter.Settings, cancellationToken);
        }

        #endregion



        List<AirspacesInformationItemModel> resultItems = new List<AirspacesInformationItemModel>();
        foreach (var item in rawGetAirspacesResponse.Items)
        {
            AirspacesInformationItemModel resultItem = ToAirspacesInformationItem(item);
            resultItems.Add(resultItem);
        }
        result.Items = resultItems.ToArray();
        return result;
    }
}
