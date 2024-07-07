namespace Fly.Models;

/// <summary>
/// Define all information about a coordinate (used to save/load documents).
/// </summary>
public class FullCoordinateInformationModel
{
    public CoordinateModel Coordinate { get; set; } = new CoordinateModel();
    public AirspacesInformationModel AirspacesInformation { get; set; } = new AirspacesInformationModel();

    /// <summary>
    /// Gets or sets the elevation, in meters.
    /// </summary>
    public virtual double? Elevation { get; set; }
    public virtual string? DisplayName { get; set; }
    public virtual string? City { get; set; }
}

