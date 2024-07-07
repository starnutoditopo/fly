namespace Fly.Models;

public class DocumentModel
{
    public DocumentModel()
    {
        Markers = [];
        Routes = [];
        Planes = [];
        FlightPlans = [];
    }

    public virtual MarkerModel[] Markers { get; set; }
    public virtual RouteModel[] Routes { get; set; }
    public virtual PlaneModel[] Planes { get; set; }
    public virtual FlightPlanModel[] FlightPlans { get; set; }
}