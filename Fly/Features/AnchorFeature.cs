using Mapsui;
using Mapsui.Nts;
using NetTopologySuite.Geometries;

namespace Fly.Features;

public class AnchorFeature : GeometryFeature
{
    public AnchorFeature(
        IFeature parentFeature,
        Geometry parentGeometry,
        int index
    )
    {
        ParentFeature = parentFeature;
        ParentGeometry = parentGeometry;
        Index = index;
    }
    public IFeature ParentFeature { get; }
    public Geometry ParentGeometry { get; }
    public int Index { get; }
}
