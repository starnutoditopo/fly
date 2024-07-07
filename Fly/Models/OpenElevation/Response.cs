using System.Collections.Generic;

namespace Fly.Models.OpenElevation;

public class Result
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }
}

public class Response
{
    public Response()
    {
        Results = new List<Result>();
    }
    public List<Result> Results { get; set; }
}

