using Fly.Models;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using DynamicData;

namespace Fly.ViewModels;

public class AirspacesInformationItemViewModel : ViewModelBase
{
    private readonly AirspacesInformationItemModel _item;
    public AirspacesInformationItemViewModel(AirspacesInformationItemModel informationItem)
    {
        _item = informationItem;

        Name = informationItem.Name;
        Type = informationItem.Type;
        IcaoClass = informationItem.IcaoClass;
        Country = informationItem.Country;
        if (informationItem.UpperLimit != null)
        {
            UpperLimit = new VerticalLimitViewModel(informationItem.UpperLimit);
        }
        if (informationItem.LowerLimit != null)
        {
            LowerLimit = new VerticalLimitViewModel(informationItem.LowerLimit);
        }
        var frequencies = new ObservableCollection<FrequencyViewModel>();
        if (informationItem.Frequencies != null)
        {
            var frequencyItems = informationItem
                .Frequencies
                .Select(f => new FrequencyViewModel(f))
                .OrderBy(f => f.Primary)
                .ThenBy(f => f.Name)
                .ThenBy(f => f.Value)
                .ToList();
            frequencies.AddRange(frequencyItems);
        }
        Frequencies = frequencies;
    }

    public FrequencyViewModel Frequency { get; set; }

    //public string Id { get; set; }

    //public string CreatedBy { get; set; }

    //public DateTimeOffset CreatedAt { get; set; }

    //public string UpdatedBy { get; set; }

    //public DateTimeOffset UpdatedAt { get; set; }

    public string Name { get; }

    //public bool DataIngestion { get; set; }

    public AirspaceType Type { get; }

    public IcaoClass IcaoClass { get; }

    //public long Activity { get; set; }

    //public bool OnDemand { get; set; }

    //public bool OnRequest { get; set; }

    //public bool ByNotam { get; set; }

    //public bool SpecialAgreement { get; set; }

    //public bool RequestCompliance { get; set; }

    //public Geometry Geometry { get; set; }

    public string Country { get; }

    public VerticalLimitViewModel UpperLimit { get; }

    public VerticalLimitViewModel LowerLimit { get; }

    //public HoursOfOperation HoursOfOperation { get; set; }

    //public long V { get; set; }

    public ObservableCollection<FrequencyViewModel> Frequencies { get; }
}
