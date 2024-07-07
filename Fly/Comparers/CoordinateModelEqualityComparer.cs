using Fly.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fly.Comparers
{
    internal class CoordinateModelEqualityComparer : EqualityComparer<CoordinateModel>
    {
        public override bool Equals(CoordinateModel? x, CoordinateModel? y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return true;
                }
                return false;
            }
            if (y == null)
            {
                return false;
            }

            if (x.Latitude == y.Latitude)
            {
                if (x.Longitude == y.Longitude)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode([DisallowNull] CoordinateModel obj)
        {
            return obj.Latitude.GetHashCode()
                ^ obj.Longitude.GetHashCode()
                ;
        }
    }
}
