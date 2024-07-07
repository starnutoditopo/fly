using System;
using System.Linq;
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
/// Elevation service based on https://github.com/Jorl17/open-elevation
/// </summary>
/// <seealso cref="IElevationService" />
public class OpenElevationService : IElevationService
{
    private readonly string? baseUrl;
    private readonly HttpClient httpClient;
    private readonly string _userAgent;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="baseUrl">URL to Nominatim service. Defaults to OSM demo site.</param>
    public OpenElevationService(
        HttpClient httpClient,
        string baseUrl = @"https://api.open-elevation.com/api/v1/lookup",
        string userAgent = "Fly"
        )
    {
        this.baseUrl = baseUrl;
        this.httpClient = httpClient;
        _userAgent = userAgent;
    }

    public async Task<double?[]> GetElevationForCoordinates(CoordinateModel[] coordinates, CancellationToken cancellationToken = default)
    {
        double?[] result = new double?[coordinates.Length];

        var requestBody = new
        {
            locations = coordinates
            .Select(x => new { latitude = x.Latitude, longitude = x.Longitude })
            .ToArray()
        };

        using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, baseUrl))
        {
            httpRequest.Headers.Add("User-Agent", new string[] { _userAgent });

            var json = JsonSerializer.Serialize(requestBody, JsonSerializationHelper.JsonSerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            httpRequest.Content = content;



            var response = await httpClient.SendAsync(httpRequest);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                throw;
            }

            Models.OpenElevation.Response? rawResponse = await response.Content.ReadFromJsonAsync<Models.OpenElevation.Response>(JsonSerializationHelper.JsonSerializerOptions, cancellationToken);
            if (rawResponse != null)
            {
                for (int i = 0; i < rawResponse.Results.Count; i++)
                {
                    var item = rawResponse.Results[i];
                    if (item != null)
                    {
                        var elevation = item.Elevation;
                        result[i] = elevation;
                    }
                }
            }
        }

        return result;
    }
}
