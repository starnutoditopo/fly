namespace Fly.Models;

public class RouteModel
{
    public RouteModel()
    {
        Coordinates = [];
        DisplayName = string.Empty;
    }

    public virtual FullCoordinateInformationModel[] Coordinates { get; set; }
    public virtual string DisplayName { get; set; }
    public virtual int Id { get; set; }
    public virtual bool IsVisible { get; set; }
}
