using Fly.Models;
using Fly.ViewModels;

namespace Fly.Extensions;

public static class CoordinateBaseViewModelExtensions
{
    //public static Geo.Coordinate ToGeoCoordinate(this CoordinateBaseViewModel coordinate)
    //{
    //    Geo.Coordinate result = new Geo.Coordinate(coordinate.Latitude, coordinate.Longitude);
    //    return result;
    //}

    public static CoordinateModel ToCoordinateModel(this CoordinateBaseViewModel coordinate)
    {
        CoordinateModel result = new CoordinateModel()
        {
            Latitude = coordinate.Latitude,
            Longitude = coordinate.Longitude
        };
        return result;
    }
}