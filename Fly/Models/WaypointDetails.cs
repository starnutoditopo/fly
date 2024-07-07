namespace Fly.Models;

public class WaypointDetails : ITimelineItem
{
    public WaypointDetails()
    {
        Coordinate = new CoordinateModel();
    }
    public CoordinateModel Coordinate { get; set; }

    /// <summary>
    /// Gets or sets the elevation, in meters.
    /// </summary>
    /// <value>
    /// The elevation, in meters.
    /// </value>
    /// <remarks>The elevation may be null, if not available. NaNs and Infinities are not supported (in compliance with json serialization).</remarks>
    public double? Elevation { get; set; }

    public GeocodingInformation? GeocodingInformation { get; set; }
}
