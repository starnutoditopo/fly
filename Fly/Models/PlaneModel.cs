namespace Fly.Models;

public class PlaneModel
{
    public PlaneModel()
    {
        DisplayName = string.Empty;
        RegistrationNumber = string.Empty;
        CruiseSpeedUnitOfMeasureCode = null;
        MeanFuelConsumptionUnitOfMeasureCode = null;
    }

    public int Id { get; set; }
    public virtual string DisplayName { get; set; }

    public virtual string RegistrationNumber { get; set; }
    public virtual double CruiseSpeed { get; set; }
    public virtual double MeanFuelConsumption { get; set; }

    public virtual string? CruiseSpeedUnitOfMeasureCode { get; set; }
    public virtual string? MeanFuelConsumptionUnitOfMeasureCode { get; set; }
}
