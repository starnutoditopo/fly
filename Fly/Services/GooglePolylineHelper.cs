using Fly.Models;
using System.Collections.Generic;

namespace Fly.Services
{
    internal static class GooglePolylineHelper
    {
        // Excerpted from https://github.com/graphhopper/graphhopper/blob/469cf3c9634440e52821832c52280ca7e28322f4/web/src/main/java/com/graphhopper/http/WebHelper.java#L47
        // See https://developers.google.com/maps/documentation/utilities/polylinealgorithm
        public static IEnumerable<Coordinate3dModel> DecodePolyline(string encoded)
        {

            int index = 0;
            int len = encoded.Length;
            int lng = 0, lat = 0, ele = 0;
            while (index < len)
            {
                // longitute
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int deltaLongitude = (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                lng += deltaLongitude;

                // latitude
                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int deltaLatitude = (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                lat += deltaLatitude;



                // elevation
                shift = 0;
                result = 0;
                do
                {
                    b = encoded[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int deltaElevation = (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                ele += deltaElevation;

                Coordinate3dModel coordinate3DModel = new Coordinate3dModel()
                {
                    Longitude = lng / 1e5,
                    Latitude = lat / 1e5,
                    Elevation = ele / 100.0
                };

                yield return coordinate3DModel;
            }
        }
    }
}
