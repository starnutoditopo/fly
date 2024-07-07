namespace Fly.Models.OpenRouteServiceElevationService;

public class LineResponse
{
    public LineResponse()
    {
        Geometry = [];
    }
    public double[][] Geometry { get; set; }
}


public class PointResponseGeometry
{
    public string Type { get; set; } = string.Empty;
    public double[] Coordinates { get; set; }= [];
}
public class PointResponse
{
    public PointResponse()
    {
        Geometry = new PointResponseGeometry();
    }
    public PointResponseGeometry Geometry { get; set; }
}

