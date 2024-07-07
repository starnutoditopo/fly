using Fly.Models.OpenAip;
using System;

namespace Fly.Models;

public enum AirspaceType
{
    Other,
    Restricted,
    Danger,
    Prohibited,
    ControlledTowerRegion, // (CTR)
    TransponderMandatoryZone, // (TMZ)
    RadioMandatoryZone,// (RMZ)
    TerminalManeuveringArea,// (TMA)
    TemporaryReservedArea,// (TRA)
    TemporarySegregatedArea,// (TSA)
    FlightInformationRegion,// (FIR)
    UpperFlightInformationRegion,// (UIR)
    AirDefenseIdentificationZone,// (ADIZ)
    AirportTrafficZone,// (ATZ)
    MilitaryAirportTrafficZone,// (MATZ)
    Airway,
    MilitaryTrainingRoute,// (MTR)
    AlertArea,
    WarningArea,
    ProtectedArea,
    HelicopterTrafficZone,// (HTZ)
    GlidingSector,
    TransponderSetting,// (TRP)
    TrafficInformationZone,// (TIZ)
    TrafficInformationArea,// (TIA)
    MilitaryTrainingArea,// (MTA)
    ControlArea,// (CTA)
    ACCSector,// (ACC)
    AerialSportingOrRecreationalActivity,
    LowAltitudeOverflightRestriction,
    MilitaryRoute,// (MRT)
    TSA_TRA_FeedingRoute,// (TFR)
    VFR_Sector,
    FIS_Sector
}

public enum VerticalLimitUnit
{
    Meter,
    Feet,
    FlightLevel
}

public enum VerticalLimitReferenceDatum
{
    GND,
    MSL,
    STD
}
public enum IcaoClass
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,

    /// <summary>
    /// The unclassified / Special Use Airspace(SUA)
    /// </summary>
    Unclassified
}

public class AirspacesInformationItemModel
{
    public string Id { get; set; }

    public string CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string UpdatedBy { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public string Name { get; set; }

    public bool DataIngestion { get; set; }

    public AirspaceType Type { get; set; }

    public IcaoClass IcaoClass { get; set; }

    public long Activity { get; set; }

    public bool OnDemand { get; set; }

    public bool OnRequest { get; set; }

    public bool ByNotam { get; set; }

    public bool SpecialAgreement { get; set; }

    public bool RequestCompliance { get; set; }

    public Geometry Geometry { get; set; }

    public string Country { get; set; }

    public VerticalLimit UpperLimit { get; set; }

    public VerticalLimit LowerLimit { get; set; }

    public HoursOfOperation HoursOfOperation { get; set; }

    public long V { get; set; }

    public Frequency[] Frequencies { get; set; }
}

public partial class VerticalLimit
{
    public long Value { get; set; }

    public VerticalLimitUnit Unit { get; set; }

    public VerticalLimitReferenceDatum ReferenceDatum { get; set; }
}


public class AirspacesInformationModel
{
    public AirspacesInformationItemModel[] Items { get; set; }
}

