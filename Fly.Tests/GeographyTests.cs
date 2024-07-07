using Fly.Extensions;

namespace Fly.Tests.Tests;


public class GeographyTests
{
    private const double NewYorkLatitude = 40.7128;
    private const double NewYorkLongitude = -74.0060;

    private const double TorontoLatitude = 43.6532;
    private const double TorontoLongitude = -79.3832;

    private const double RomeLatitude = 41.902782;
    private const double RomeLongitude = 12.496366;

    private const double MadridLatitude = 40.4168;
    private const double MadridLongitude = -3.7038;

    private const double LondonLatitude = 51.5072;
    private const double LondonLongitude = -0.1276;

    [Fact]
    public void TestRhumbBearingNewYorkToToronto()
    {
        double rhumbBearing = GeographyExtensions.GetRhumbBearing(NewYorkLongitude, NewYorkLatitude, TorontoLongitude, TorontoLatitude);
        Assert.Equal(306, (int)rhumbBearing);
    }

    [Fact]
    public void TestRhumbDistanceNewYorkToToronto()
    {
        double rhumbDistance = GeographyExtensions.GetRhumbDistance(NewYorkLongitude, NewYorkLatitude, TorontoLongitude, TorontoLatitude);
        Assert.Equal(550, (int)(rhumbDistance / 1000));
    }

    [Fact]
    public void TestRhumbBearingNewYorkToRome()
    {
        double rhumbBearing = GeographyExtensions.GetRhumbBearing(NewYorkLongitude, NewYorkLatitude, RomeLongitude, RomeLatitude);
        Assert.Equal(88, (int)rhumbBearing);
    }

    [Fact]
    public void TestRhumbDistanceNewYorkToRome()
    {
        double rhumbDistance = GeographyExtensions.GetRhumbDistance(NewYorkLongitude, NewYorkLatitude, RomeLongitude, RomeLatitude);
        Assert.Equal(7226, (int)(rhumbDistance / 1000));
    }

    [Fact]
    public void TestRhumbBearingNewYorkToMadrid()
    {
        double rhumbBearing = GeographyExtensions.GetRhumbBearing(NewYorkLongitude, NewYorkLatitude, MadridLongitude, MadridLatitude);
        Assert.Equal(90, (int)rhumbBearing);
    }

    [Fact]
    public void TestRhumbDistanceNewYorkToMadrid()
    {
        double rhumbDistance = GeographyExtensions.GetRhumbDistance(NewYorkLongitude, NewYorkLatitude, MadridLongitude, MadridLatitude);
        Assert.Equal(5938, (int)(rhumbDistance / 1000));
    }

    [Fact]
    public void TestRhumbBearingNewYorkToLondon()
    {
        double rhumbBearing = GeographyExtensions.GetRhumbBearing(NewYorkLongitude, NewYorkLatitude, LondonLongitude, LondonLatitude);
        Assert.Equal(78, (int)rhumbBearing);
    }

    [Fact]
    public void TestRhumbDistanceNewYorkToLondon()
    {
        double rhumbDistance = GeographyExtensions.GetRhumbDistance(NewYorkLongitude, NewYorkLatitude, LondonLongitude, LondonLatitude);
        Assert.Equal(5794, (int)(rhumbDistance / 1000));
    }
}
