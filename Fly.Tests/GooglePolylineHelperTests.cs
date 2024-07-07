using Fly.Services;

namespace Fly.Tests.Tests;

public class GooglePolylineHelperTests
{
    [Fact]
    public void TestGooglePolylineHelperResult2()
    {
        // Call to:
        //  https://api.openrouteservice.org/elevation/line
        // Verb:
        //  POST
        // body:
        //  {
        //      "format_in": "polyline",
        //      "format_out": "encodedpolyline5",
        //      "geometry": [
        //          [13.349762, 38.112952],
        //          [12.638397, 37.645772]
        //      ]
        //  }
        // Result:

        string s = "}|rgF_knpAolFzfzA~|iCf{C";
        var coordinates = GooglePolylineHelper
            .DecodePolyline(s)
            .ToArray();

        Assert.Equal(2, coordinates.Length);

        Assert.Equal(38.11295, coordinates[0].Longitude);
        Assert.Equal(13.34976, coordinates[0].Latitude);
        Assert.Equal(38, coordinates[0].Elevation);

        Assert.Equal(37.64577, coordinates[1].Longitude);
        Assert.Equal(12.6384, coordinates[1].Latitude);
        Assert.Equal(13, coordinates[1].Elevation);
    }
}
