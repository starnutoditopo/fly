namespace Fly.Models;

public class FlightPlanModel
{
    public FlightPlanModel()
    {
        DisplayName = string.Empty;
    }

    public virtual string DisplayName { get; set; }
    public int RouteId { get; set; }
    public int PlaneId { get; set; }
    public int Id { get; set; }
    public int ResidualAutonomy { get; set; }
    public int TimeToReachAlternateField { get; set; }
}