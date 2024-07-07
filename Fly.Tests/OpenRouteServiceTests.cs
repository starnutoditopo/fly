﻿using System.Text.Json;
using Fly.Models.Nominatim;

namespace Fly.Tests.Tests;

public class OpenRouteServiceTests
{
    [Fact]
    public void TestReverseGeocoding()
    {
        // Response from: (GET) https://api.openrouteservice.org/geocode/reverse?api_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&point.lon=2.294471&point.lat=48.858268
        string response = @"

{
  ""geocoding"": {
    ""version"": ""0.2"",
    ""attribution"": ""https://openrouteservice.org/terms-of-service/#attribution-geocode"",
    ""query"": {
      ""size"": 10,
      ""private"": false,
      ""point.lat"": 48.858268,
      ""point.lon"": 2.294471,
      ""boundary.circle.lat"": 48.858268,
      ""boundary.circle.lon"": 2.294471,
      ""lang"": {
        ""name"": ""Italian"",
        ""iso6391"": ""it"",
        ""iso6393"": ""ita"",
        ""via"": ""header"",
        ""defaulted"": false
      },
      ""querySize"": 20
    },
    ""engine"": {
      ""name"": ""Pelias"",
      ""author"": ""Mapzen"",
      ""version"": ""1.0""
    },
    ""timestamp"": 1717237754646
  },
  ""type"": ""FeatureCollection"",
  ""features"": [
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.2945,
          48.85826
        ]
      },
      ""properties"": {
        ""id"": ""relation/4114842"",
        ""gid"": ""openstreetmap:venue:relation/4114842"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""relation/4114842"",
        ""name"": ""Tour Eiffel 2e étage"",
        ""confidence"": 0.9,
        ""distance"": 0.002,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Tour Eiffel 2e étage, Parigi, Francia""
      },
      ""bbox"": [
        2.2941111,
        48.8580066,
        2.2948853,
        48.8585159
      ]
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.294493,
          48.85824
        ]
      },
      ""properties"": {
        ""id"": ""node/4013678139"",
        ""gid"": ""openstreetmap:venue:node/4013678139"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""node/4013678139"",
        ""name"": ""75056Y - A"",
        ""confidence"": 0.9,
        ""distance"": 0.004,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""75056Y - A, Parigi, Francia""
      }
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.294515,
          48.858251
        ]
      },
      ""properties"": {
        ""id"": ""relation/4114841"",
        ""gid"": ""openstreetmap:venue:relation/4114841"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""relation/4114841"",
        ""name"": ""Tour Eiffel 3e étage"",
        ""confidence"": 0.9,
        ""distance"": 0.004,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Tour Eiffel 3e étage, Parigi, Francia"",
        ""addendum"": {
          ""osm"": {
            ""wheelchair"": ""limited""
          }
        }
      },
      ""bbox"": [
        2.2943341,
        48.8581548,
        2.2946601,
        48.8583693
      ]
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.294531,
          48.858291
        ]
      },
      ""properties"": {
        ""id"": ""node/3134285383"",
        ""gid"": ""openstreetmap:venue:node/3134285383"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""node/3134285383"",
        ""name"": ""Bureau de Gustave Eiffel"",
        ""confidence"": 0.9,
        ""distance"": 0.005,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Bureau de Gustave Eiffel, Parigi, Francia"",
        ""addendum"": {
          ""osm"": {
            ""wheelchair"": ""yes""
          }
        }
      }
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.293923,
          48.858389
        ]
      },
      ""properties"": {
        ""id"": ""node/10637250505"",
        ""gid"": ""openstreetmap:venue:node/10637250505"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""node/10637250505"",
        ""name"": ""Tour Eiffel"",
        ""confidence"": 0.8,
        ""distance"": 0.042,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Tour Eiffel, Parigi, Francia""
      }
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.294497,
          48.858133
        ]
      },
      ""properties"": {
        ""id"": ""node/3135278479"",
        ""gid"": ""openstreetmap:venue:node/3135278479"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""node/3135278479"",
        ""name"": ""Le Jules Verne"",
        ""street"": ""Avenue Gustave Eiffel"",
        ""postalcode"": ""75007"",
        ""confidence"": 0.8,
        ""distance"": 0.015,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Le Jules Verne, Parigi, Francia"",
        ""addendum"": {
          ""osm"": {
            ""wheelchair"": ""yes"",
            ""wikidata"": ""Q3223818"",
            ""website"": ""https://www.lejulesverne-paris.com""
          }
        }
      }
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.29424,
          48.858262
        ]
      },
      ""properties"": {
        ""id"": ""node/3135214938"",
        ""gid"": ""openstreetmap:venue:node/3135214938"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""node/3135214938"",
        ""name"": ""Buffets"",
        ""confidence"": 0.8,
        ""distance"": 0.017,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Buffets, Parigi, Francia""
      }
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.294265,
          48.858411
        ]
      },
      ""properties"": {
        ""id"": ""way/308145239"",
        ""gid"": ""openstreetmap:venue:way/308145239"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""way/308145239"",
        ""name"": ""58 tour Eiffel"",
        ""confidence"": 0.8,
        ""distance"": 0.022,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""58 tour Eiffel, Parigi, Francia""
      },
      ""bbox"": [
        2.2940699,
        48.8582817,
        2.2944601,
        48.858541
      ]
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.294253,
          48.858102
        ]
      },
      ""properties"": {
        ""id"": ""way/308145259"",
        ""gid"": ""openstreetmap:venue:way/308145259"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""way/308145259"",
        ""name"": ""Padiglione Ferrié"",
        ""confidence"": 0.8,
        ""distance"": 0.024,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Padiglione Ferrié, Parigi, Francia""
      },
      ""bbox"": [
        2.2940646,
        48.8579786,
        2.2944415,
        48.8582259
      ]
    },
    {
      ""type"": ""Feature"",
      ""geometry"": {
        ""type"": ""Point"",
        ""coordinates"": [
          2.294729,
          48.858422
        ]
      },
      ""properties"": {
        ""id"": ""way/308145258"",
        ""gid"": ""openstreetmap:venue:way/308145258"",
        ""layer"": ""venue"",
        ""source"": ""openstreetmap"",
        ""source_id"": ""way/308145258"",
        ""name"": ""Padiglione Eiffel"",
        ""confidence"": 0.8,
        ""distance"": 0.026,
        ""accuracy"": ""point"",
        ""country"": ""Francia"",
        ""country_gid"": ""whosonfirst:country:85633147"",
        ""country_a"": ""FRA"",
        ""macroregion"": ""Île-de-France"",
        ""macroregion_gid"": ""whosonfirst:macroregion:404227465"",
        ""macroregion_a"": ""IF"",
        ""region"": ""Parigi"",
        ""region_gid"": ""whosonfirst:region:85683497"",
        ""region_a"": ""VP"",
        ""localadmin"": ""Paris"",
        ""localadmin_gid"": ""whosonfirst:localadmin:1159322569"",
        ""locality"": ""Parigi"",
        ""locality_gid"": ""whosonfirst:locality:101751119"",
        ""borough"": ""7th Arrondissement"",
        ""borough_gid"": ""whosonfirst:borough:1158894245"",
        ""neighbourhood"": ""Gros Caillou"",
        ""neighbourhood_gid"": ""whosonfirst:neighbourhood:85873841"",
        ""continent"": ""Europa"",
        ""continent_gid"": ""whosonfirst:continent:102191581"",
        ""label"": ""Padiglione Eiffel, Parigi, Francia""
      },
      ""bbox"": [
        2.294541,
        48.8582986,
        2.294918,
        48.8585458
      ]
    }
  ],
  ""bbox"": [
    2.293923,
    48.8579786,
    2.294918,
    48.8585458
  ]
}

";
        var result = JsonSerializer.Deserialize<Fly.Models.OpenRouteServiceReverseGeocodeService.Response>(response, JsonSerializationHelper.JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.Equal("Paris", result.features[0].properties.localadmin);
        Assert.Equal("Tour Eiffel 2e étage", result.features[0].properties.name);
    }

    [Fact]
    public void TestReverseGeocoding2()
    {
        // Response from: (GET) https://api.openrouteservice.org/geocode/reverse?api_key=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx&point.lon=8.184032097416804&point.lat=44.758578501910506&size=1
        string response = @"

{
   ""geocoding"":{
      ""version"":""0.2"",
      ""attribution"":""https://openrouteservice.org/terms-of-service/#attribution-geocode"",
      ""query"":{
         ""size"":1,
         ""private"":false,
         ""point.lat"":44.758578501910506,
         ""point.lon"":8.184032097416804,
         ""boundary.circle.lat"":44.758578501910506,
         ""boundary.circle.lon"":8.184032097416804,
         ""lang"":{
            ""name"":""English"",
            ""iso6391"":""en"",
            ""iso6393"":""eng"",
            ""via"":""default"",
            ""defaulted"":true
         },
         ""querySize"":2
      },
      ""engine"":{
         ""name"":""Pelias"",
         ""author"":""Mapzen"",
         ""version"":""1.0""
      },
      ""timestamp"":1717237854893
   },
   ""type"":""FeatureCollection"",
   ""features"":[
      {
         ""type"":""Feature"",
         ""geometry"":{
            ""type"":""Point"",
            ""coordinates"":[
               8.183494,
               44.758049
            ]
         },
         ""properties"":{
            ""id"":""way/244661604"",
            ""gid"":""openstreetmap:venue:way/244661604"",
            ""layer"":""venue"",
            ""source"":""openstreetmap"",
            ""source_id"":""way/244661604"",
            ""name"":""Aviosuperficie di Boglietto"",
            ""confidence"":0.8,
            ""distance"":0.073,
            ""accuracy"":""point"",
            ""country"":""Italy"",
            ""country_gid"":""whosonfirst:country:85633253"",
            ""country_a"":""ITA"",
            ""macroregion"":""Piedmont"",
            ""macroregion_gid"":""whosonfirst:macroregion:404227493"",
            ""region"":""Asti"",
            ""region_gid"":""whosonfirst:region:85685195"",
            ""region_a"":""AT"",
            ""localadmin"":""Costigliole D'Asti"",
            ""localadmin_gid"":""whosonfirst:localadmin:404456021"",
            ""continent"":""Europe"",
            ""continent_gid"":""whosonfirst:continent:102191581"",
            ""label"":""Aviosuperficie di Boglietto, Costigliole D'Asti, AT, Italy"",
            ""addendum"":{
               ""osm"":{
                  ""website"":""http://www.aviosuperficieboglietto.it""
               }
            }
         },
         ""bbox"":[
            8.178605,
            44.7567657,
            8.1883142,
            44.7592165
         ]
      }
   ],
   ""bbox"":[
      8.178605,
      44.7567657,
      8.1883142,
      44.7592165
   ]
}
";
        var result = JsonSerializer.Deserialize<Fly.Models.OpenRouteServiceReverseGeocodeService.Response>(response, JsonSerializationHelper.JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.Equal("Costigliole D'Asti", result.features[0].properties.localadmin);
        Assert.Equal("Aviosuperficie di Boglietto", result.features[0].properties.name);
    }

    [Fact]
    public void TestElevationService()
    {
        // Response from: (POST) https://api.openrouteservice.org/elevation/point
        // headers: Authorization xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        // body:
        //  {
        //      "format_in": "point",
        //      "geometry": [8.184032097416804,44.758578501910506]
        //  }
        string response = @"
{
    ""attribution"": ""service by https://openrouteservice.org | data by http://srtm.csi.cgiar.org"",
    ""geometry"": {
        ""coordinates"": [
            8.184032,
            44.758579,
            188
        ],
        ""type"": ""Point""
    },
    ""timestamp"": 1717260522,
    ""version"": ""0.2.1""
}

";
        var result = JsonSerializer.Deserialize<Fly.Models.OpenRouteServiceElevationService.PointResponse>(response, JsonSerializationHelper.JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.NotNull(result.Geometry);
        Assert.NotNull(result.Geometry.Type);
        Assert.NotNull(result.Geometry.Coordinates);
        Assert.Equal("Point", result.Geometry.Type);
        Assert.Equal(3, result.Geometry.Coordinates.Length);
        Assert.Equal(188.0, result.Geometry.Coordinates[2]);
    }
}
