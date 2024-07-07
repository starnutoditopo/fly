using Fly.Models.OpenAip;

namespace Fly.ViewModels;

public class FrequencyViewModel: ViewModelBase
{
    private readonly Frequency _frequency;
    public FrequencyViewModel(Frequency frequency)
    {
        _frequency = frequency;
        Value = frequency.Value;
        Unit = frequency.Unit;
        Remarks = frequency.Remarks;
        Primary = frequency.Primary;
        Name = frequency.Name;        
    }

    public string Value { get; set; }

    public bool Primary { get; set; }

    public long Unit { get; set; }

    public string Name { get; set; }

    public string Remarks { get; set; }

    //public string Id { get; set; }
}
