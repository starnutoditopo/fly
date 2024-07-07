using System;

namespace Fly.Extensions;

// See: https://stackoverflow.com/a/2042883/1288109
// See: http://www.movable-type.co.uk/scripts/latlong.html
public static class GeographyExtensions
{
    /// <summary>
    /// Gets the rhumb bearing, in degrees (0 to 360), from location1 to location2, respect to the North, clock-wise.
    /// </summary>
    /// <param name="location1Longitude">The location1 longitude.</param>
    /// <param name="location1Latitude">The location1 latitude.</param>
    /// <param name="location2Longitude">The location2 longitude.</param>
    /// <param name="location2Latitude">The location2 latitude.</param>
    /// <returns>Tthe rhumb bearing, in degrees (0 to 360), from location1 to location2, respect to the North, clock-wise.</returns>
    public static double GetRhumbBearing(
        double location1Longitude,
        double location1Latitude,
        double location2Longitude,
        double location2Latitude
        )
    {
        var lat1 = ConvertDegreesToRadians(location1Latitude);
        var lat2 = ConvertDegreesToRadians(location2Latitude);
        var dLon = ConvertDegreesToRadians(location2Longitude - location1Longitude);

        var dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
        if (Math.Abs(dLon) > Math.PI) dLon = (dLon > 0) ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
        var brng = Math.Atan2(dLon, dPhi);

        return (ConvertRadiansToDegrees(brng) + 360) % 360;
    }

    public static double ConvertDegreesToRadians(double angleInDegrees)
    {
        return angleInDegrees * (Math.PI / 180);
    }
    public static double ConvertRadiansToDegrees(double angleInRadians)
    {
        return 180.0 * angleInRadians / Math.PI;
    }

    /// <summary>
    /// Gets the Earth's mean radius using the WGS84 ellipsoid (in meters).
    /// </summary>
    /// <value>
    /// The Earth's mean radius using the WGS84 ellipsoid (in meters).
    /// </value>
    /// <remarks>
    /// See: https://github.com/openlayers/openlayers/blob/main/src/ol/sphere.js
    /// </remarks>
    public static double EarthRadiusInMeters => 6371008.8;

    /// <summary>
    /// Get the rhumb distance (in meters) between two geographic coordinates.
    /// </summary>
    /// <param name="location1Latitude">Location 1's latitude.</param>
    /// <param name="location1Longitude">Location 1's longitude.</param>
    /// <param name="location2Latitude">Location 2's latitude.</param>
    /// <param name="location2Longitude">Location 2's longitude.</param>
    /// <returns>The rhumb distance (in meters) between two geographic coordinates.</returns>
    public static double GetRhumbDistance(
        double location1Longitude,
        double location1Latitude,
        double location2Longitude,
        double location2Latitude
    )
    {
        double R = EarthRadiusInMeters;
        var lat1 = ConvertDegreesToRadians(location1Latitude);
        var lat2 = ConvertDegreesToRadians(location2Latitude);
        var dLat = ConvertDegreesToRadians(location2Latitude - location1Latitude);
        var dLon = ConvertDegreesToRadians(Math.Abs(location2Longitude - location1Longitude));

        var dPhi = Math.Log(Math.Tan(lat2 / 2 + Math.PI / 4) / Math.Tan(lat1 / 2 + Math.PI / 4));
        var q = Math.Cos(lat1);
        if (dPhi != 0) q = dLat / dPhi;  // E-W line gives dPhi=0
                                         // if dLon over 180° take shorter rhumb across 180° meridian:
        if (dLon > Math.PI)
        {
            dLon = 2 * Math.PI - dLon;
        }
        var dist = Math.Sqrt(dLat * dLat + q * q * dLon * dLon) * R;
        return dist;
    }

    /// <summary>
    /// Get the great circle distance (in meters) between two geographic coordinates.
    /// </summary>
    /// <param name="location1Latitude">Location 1's latitude.</param>
    /// <param name="location1Longitude">Location 1's longitude.</param>
    /// <param name="location2Latitude">Location 2's latitude.</param>
    /// <param name="location2Longitude">Location 2's longitude.</param>
    /// <returns>The great circle distance (in meters) between two geographic coordinates.</returns>
    /// <remarks>See: https://github.com/openlayers/openlayers/blob/main/src/ol/sphere.js </remarks>
    public static double GetDistance(
        double location1Longitude,
        double location1Latitude,
        double location2Longitude,
        double location2Latitude
    )
    {
        double R = EarthRadiusInMeters;
        var lat1 = ConvertDegreesToRadians(location1Latitude);
        var lat2 = ConvertDegreesToRadians(location2Latitude);
        var deltaLatBy2 = (lat2 - lat1) / 2;
        var deltaLonBy2 = ConvertDegreesToRadians(location2Longitude - location1Longitude) / 2;
        var a =
          Math.Sin(deltaLatBy2) * Math.Sin(deltaLatBy2) +
          Math.Sin(deltaLonBy2) *
            Math.Sin(deltaLonBy2) *
            Math.Cos(lat1) *
            Math.Cos(lat2);
        return 2 * R * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
