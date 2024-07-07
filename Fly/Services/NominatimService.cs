using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Fly.Models;
using Fly.Models.Nominatim;

namespace Fly.Services;
public class NominatimService : IReverseGeocodeService
{
    private readonly string? baseUrl;
    private readonly ReverseGeocodeDetailLevel detailLevel;
    private readonly HttpClient httpClient;
    private readonly string _userAgent;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="baseUrl">URL to Nominatim service. Defaults to OSM demo site.</param>
    public NominatimService(
        HttpClient httpClient,
        string baseUrl = @"https://nominatim.openstreetmap.org/reverse",
        string userAgent = "Fly",
        ReverseGeocodeDetailLevel detailLevel = ReverseGeocodeDetailLevel.Building
        )
    {
        this.baseUrl = baseUrl;
        this.detailLevel = detailLevel;
        this.httpClient = httpClient;
        _userAgent = userAgent;
    }

    public async Task<GeocodingInformation[]> GetGeocodeInformationForCoordinates(CoordinateModel[] coordinates, CancellationToken cancellationToken = default)
    {
        GeocodingInformation[] result = new GeocodingInformation[coordinates.Length];
        string format = "geocodejson"; //"jsonv2";
        for (int i = 0; i < coordinates.Length; i++)
        {
            var coordinate = coordinates[i];

            string url = $"{baseUrl}?format={format}&lat={coordinate.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={coordinate.Longitude.ToString(CultureInfo.InvariantCulture)}&zoom={((int)detailLevel).ToString(CultureInfo.InvariantCulture)}&addressdetails={1.ToString(CultureInfo.InvariantCulture)}";
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
            {
                httpRequest.Headers.Add("User-Agent", new string[] { _userAgent });

                var response = await httpClient.SendAsync(httpRequest);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception)
                {
                    throw;
                }

                Coordinate? nominatimCoordinate = await response.Content.ReadFromJsonAsync<Coordinate>(JsonSerializationHelper.JsonSerializerOptions, cancellationToken);
                var geocoding = nominatimCoordinate?.Features?.FirstOrDefault()?.Properties?.Geocoding;
                string? name = geocoding?.Name;
                string? city = geocoding?.City;
                GeocodingInformation geocodingInformation = new GeocodingInformation()
                {
                    DisplayName = name,
                    City = city,
                };
                result[i] = geocodingInformation;
            }
        }

        return result;
    }
}
