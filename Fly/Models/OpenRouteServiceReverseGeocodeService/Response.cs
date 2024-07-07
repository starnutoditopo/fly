using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fly.Models.OpenRouteServiceReverseGeocodeService;

public class Addendum
{
    public Osm osm { get; set; }
}

public class Engine
{
    public string name { get; set; }
    public string author { get; set; }
    public string version { get; set; }
}

public class Feature
{
    public string type { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }
    public List<double> bbox { get; set; }
}

public class Geocoding
{
    public string version { get; set; }
    public string attribution { get; set; }
    public Query query { get; set; }
    public Engine engine { get; set; }
    public long timestamp { get; set; }
}

public class Geometry
{
    public string type { get; set; }
    public List<double> coordinates { get; set; }
}

public class Lang
{
    public string name { get; set; }
    public string iso6391 { get; set; }
    public string iso6393 { get; set; }
    public string via { get; set; }
    public bool defaulted { get; set; }
}

public class Osm
{
    public string website { get; set; }
}

public class Properties
{
    public string id { get; set; }
    public string gid { get; set; }
    public string layer { get; set; }
    public string source { get; set; }
    public string source_id { get; set; }
    public string name { get; set; }
    public double confidence { get; set; }
    public double distance { get; set; }
    public string accuracy { get; set; }
    public string country { get; set; }
    public string country_gid { get; set; }
    public string country_a { get; set; }
    public string macroregion { get; set; }
    public string macroregion_gid { get; set; }
    public string region { get; set; }
    public string region_gid { get; set; }
    public string region_a { get; set; }
    public string localadmin { get; set; }
    public string localadmin_gid { get; set; }
    public string continent { get; set; }
    public string continent_gid { get; set; }
    public string label { get; set; }
    public Addendum addendum { get; set; }
}

public class Query
{
    public int size { get; set; }
    public bool @private { get; set; }

    [JsonPropertyName("point.lat")]
    public double pointlat { get; set; }

    [JsonPropertyName("point.lon")]
    public double pointlon { get; set; }

    [JsonPropertyName("boundary.circle.lat")]
    public double boundarycirclelat { get; set; }

    [JsonPropertyName("boundary.circle.lon")]
    public double boundarycirclelon { get; set; }
    public Lang lang { get; set; }
    public int querySize { get; set; }
}

public class Response
{
    public Geocoding geocoding { get; set; }
    public string type { get; set; }
    public List<Feature> features { get; set; }
    public List<double> bbox { get; set; }
}

