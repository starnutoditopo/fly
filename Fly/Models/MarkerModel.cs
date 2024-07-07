namespace Fly.Models;

public class MarkerModel
{
    public MarkerModel()
    {
    }

    public virtual FullCoordinateInformationModel FullCoordinateInformationModel { get; set; } = null!;
    public int Id { get; set; }
    public virtual bool IsVisible { get; set; }
}
