using System.Text.Json;
using Fly.Models.Nominatim;

namespace Fly.Tests.Tests;

public class OpenElevationTests
{
    [Fact]
    public void TestOpenElevationResult()
    {
        // Response from: https://api.open-elevation.com/api/v1/lookup?locations=41.161758,-8.583933
        string openElevationResponse = @"
{
  ""results"": [
    {
      ""latitude"": 41.161758,
      ""longitude"": -8.583933,
      ""elevation"": 117.0
    }
  ]
}";


        var result = JsonSerializer.Deserialize<Fly.Models.OpenElevation.Response>(openElevationResponse, JsonSerializationHelper.JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.NotNull(result.Results);
        Assert.Single(result.Results);
        Assert.Equal(117.0, result.Results[0].Elevation);
    }
}
