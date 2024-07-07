using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Fly.Models;
using Fly.Models.Nominatim;

namespace Fly.Services;

/// <summary>
/// Elevation service based on OpenRouteService https://github.com/GIScience/openelevationservice
/// See https://openrouteservice.org/services/
/// </summary>
/// <seealso cref="IElevationService" />
partial class OpenRouteService : IElevationService
{
    public async Task<double?[]> GetElevationForCoordinates(CoordinateModel[] coordinates, CancellationToken cancellationToken = default)
    {
        if (coordinates.Length == 0)
        {
            return [];
        }
        if (coordinates.Length == 1)
        {
            var elevation = await Private_GetElevationForSingleCoordinate(coordinates[0], cancellationToken);
            return [elevation];
        }
        return await Private_GetElevationForCoordinates(coordinates, cancellationToken);
    }

    private async Task<double?[]> Private_GetElevationForCoordinates(CoordinateModel[] coordinates, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    private async Task<double?> Private_GetElevationForSingleCoordinate(CoordinateModel coordinate, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            format_in = "point",
            geometry = new double[] { coordinate.Longitude, coordinate.Latitude }
        };

        string apiKey = _settingsService.GetOpenRouteService_ApiKey();
        string baseUrl = _settingsService.GetOpenRouteService_Api_BaseUrl();

        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/elevation/point"))
        {
            httpRequest.Headers.Add("Authorization", new string[] { apiKey });

            var json = JsonSerializer.Serialize(requestBody, JsonSerializationHelper.JsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpRequest.Content = content;

            var response = await _httpClient.SendAsync(httpRequest);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw;
            }

            Models.OpenRouteServiceElevationService.PointResponse? rawResponse = await response.Content.ReadFromJsonAsync<Models.OpenRouteServiceElevationService.PointResponse>(JsonSerializationHelper.JsonSerializerOptions, cancellationToken);
            if (rawResponse != null)
            {
                if (rawResponse.Geometry != null)
                {
                    if (rawResponse.Geometry.Type == "Point")
                    {
                        return rawResponse.Geometry.Coordinates[2];
                    }
                }
            }
        }

        return null;
    }
}
