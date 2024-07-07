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

/// <summary>
/// Reverse geocoding service based on OpenRouteService https://openrouteservice.org/dev/#/api-docs/geocode/reverse/get
/// See also: https://github.com/pelias/documentation/blob/master/reverse.md#reverse-geocoding
/// </summary>
/// <seealso cref="IReverseGeocodeService" />
partial class OpenRouteService : IReverseGeocodeService
{
    public async Task<GeocodingInformation[]> GetGeocodeInformationForCoordinates(CoordinateModel[] coordinates, CancellationToken cancellationToken = default)
    {
        if (coordinates.Length > 1)
        {
            throw new NotImplementedException();
        }
        
        if (coordinates.Length == 1)
        {
            var coordinate = coordinates[0];
            string apiKey = _settingsService.GetOpenRouteService_ApiKey();
            string baseUrl = _settingsService.GetOpenRouteService_Api_BaseUrl();

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/geocode/reverse?api_key={apiKey}&point.lon={coordinate.Longitude.ToString(CultureInfo.InvariantCulture)}&point.lat={coordinate.Latitude.ToString(CultureInfo.InvariantCulture)}&size=1"))
            {
                var response = await _httpClient.SendAsync(httpRequest);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception)
                {
                    throw;
                }

                Models.OpenRouteServiceReverseGeocodeService.Response? rawResponse = await response.Content.ReadFromJsonAsync<Models.OpenRouteServiceReverseGeocodeService.Response>(JsonSerializationHelper.JsonSerializerOptions, cancellationToken);
                if (rawResponse != null)
                {
                    if (rawResponse.features != null)
                    {
                        if (rawResponse.features.Any())
                        {
                            var feature = rawResponse.features[0];
                            if (feature.type == "Feature")
                            {
                                var properties = feature.properties;
                                if(properties != null){
                                    GeocodingInformation geocodingInformation = new GeocodingInformation();
                                    geocodingInformation.DisplayName = properties.name;
                                    geocodingInformation.City = properties.localadmin;
                                    return [geocodingInformation];
                                }
                            }
                        }
                    }
                }
            }
        }
        return [];
    }
}
