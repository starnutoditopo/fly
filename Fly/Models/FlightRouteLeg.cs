namespace Fly.Models;

/// <summary>
/// Represents a leg of a route. A route leg is the part of the route between two stop waypoints.
/// </summary>
public class FlightRouteLeg
{
    /// <summary>
    /// Gets or sets the rhumb distance, in Km.
    /// </summary>
    /// <value>
    /// The rhumb distance, in Km.
    /// </value>
    public double RhumbDistance { get; set; }

    /// <summary>
    /// Gets or sets the rhumb bearing.
    /// </summary>
    /// <value>
    /// The rhumb bearing.
    /// </value>
    public double RhumbBearing { get; set; }

    /// <summary>
    /// Gets or sets the (orthodromic) distance, in Km.
    /// </summary>
    /// <value>
    /// The (orthodromic) distance, in Km.
    /// </value>
    public double Distance { get; set; }

    /// <summary>
    /// Gets or sets the fuel consumption, in l.
    /// </summary>
    /// <value>
    /// The fuel consumption, in l.
    /// </value>
    public double FuelConsumption { get; set; }
    /// <summary>
    /// Gets or sets the estimated time, in minutes.
    /// </summary>
    /// <value>
    /// The estimated time, in minutes.
    /// </value>
    public double Time { get; set; }
}
