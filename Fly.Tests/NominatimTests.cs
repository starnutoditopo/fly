using System.Text.Json;
using Fly.Models.Nominatim;

namespace Fly.Tests.Tests;

public class NominatimTests
{
    [Fact]
    public void TestNominatimResult1()
    {
        // Response from: https://nominatim.openstreetmap.org/reverse?format=geocodejson&lat=44.758578501910506&lon=8.184032097416804&zoom=10&addressdetails=1
        string nominatimResponse = @"
{
  ""type"": ""FeatureCollection"",
  ""geocoding"": {
    ""version"": ""0.1.0"",
    ""attribution"": ""Data © OpenStreetMap contributors, ODbL 1.0. http://osm.org/copyright"",
    ""licence"": ""ODbL"",
    ""query"": """"
  },
  ""features"": [
    {
      ""type"": ""Feature"",
      ""properties"": {
        ""geocoding"": {
          ""place_id"": 96356617,
          ""osm_type"": ""relation"",
          ""osm_id"": 43571,
          ""osm_key"": ""boundary"",
          ""osm_value"": ""administrative"",
          ""type"": ""city"",
          ""accuracy"": 0,
          ""label"": ""Costigliole d'Asti, Asti, Piemonte, 14055, Italia"",
          ""name"": ""Costigliole d'Asti"",
          ""postcode"": ""14055"",
          ""county"": ""Asti"",
          ""state"": ""Piemonte"",
          ""country"": ""Italia"",
          ""country_code"": ""it"",
          ""admin"": {
            ""level8"": ""Costigliole d'Asti"",
            ""level6"": ""Asti"",
            ""level4"": ""Piemonte""
          }
        }
      },
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          8.1822235,
          44.7864182
        ]
      }
    }
  ]
}";


        var result = JsonSerializer.Deserialize<Coordinate>(nominatimResponse, JsonSerializationHelper.JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.Equal("Costigliole d'Asti, Asti, Piemonte, 14055, Italia", result.Features[0].Properties.Geocoding.Label);
        Assert.Equal("Costigliole d'Asti", result.Features[0].Properties.Geocoding.Name);
    }

    [Fact]
    public void TestNominatimResult2()
    {
        // Response from: https://nominatim.openstreetmap.org/reverse?format=geocodejson&lat=44.758578501910506&lon=8.184032097416804&zoom=18&addressdetails=1
        string nominatimResponse = @"
{
  ""type"": ""FeatureCollection"",
  ""geocoding"": {
    ""version"": ""0.1.0"",
    ""attribution"": ""Data © OpenStreetMap contributors, ODbL 1.0. http://osm.org/copyright"",
    ""licence"": ""ODbL"",
    ""query"": """"
  },
  ""features"": [
    {
      ""type"": ""Feature"",
      ""properties"": {
        ""geocoding"": {
          ""place_id"": 96196792,
          ""osm_type"": ""way"",
          ""osm_id"": 244661604,
          ""osm_key"": ""aeroway"",
          ""osm_value"": ""aerodrome"",
          ""type"": ""house"",
          ""accuracy"": 0,
          ""label"": ""Aviosuperficie di Boglietto, Strada Crossa, Costigliole d'Asti, Asti, Piemonte, 10101, Italia"",
          ""name"": ""Aviosuperficie di Boglietto"",
          ""postcode"": ""10101"",
          ""street"": ""Strada Crossa"",
          ""city"": ""Costigliole d'Asti"",
          ""county"": ""Asti"",
          ""state"": ""Piemonte"",
          ""country"": ""Italia"",
          ""country_code"": ""it"",
          ""admin"": {
            ""level8"": ""Costigliole d'Asti"",
            ""level6"": ""Asti"",
            ""level4"": ""Piemonte""
          }
        }
      },
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          8.18540681446492,
          44.757855
        ]
      }
    }
  ]
}";
        var result = JsonSerializer.Deserialize<Coordinate>(nominatimResponse, JsonSerializationHelper.JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.Equal("Aviosuperficie di Boglietto, Strada Crossa, Costigliole d'Asti, Asti, Piemonte, 10101, Italia", result.Features[0].Properties.Geocoding.Label);
        Assert.Equal("Aviosuperficie di Boglietto", result.Features[0].Properties.Geocoding.Name);
        Assert.Equal("Costigliole d'Asti", result.Features[0].Properties.Geocoding.City);
    }
}
