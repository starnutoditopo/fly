using Fly.Models;

namespace Fly.ViewModels;

public class VerticalLimitViewModel : ViewModelBase
{
    private readonly VerticalLimit _verticalLimit;
    public VerticalLimitViewModel(VerticalLimit limit)
    {
        _verticalLimit = limit;

        Value = limit.Value;
        Unit = limit.Unit;
        ReferenceDatum = limit.ReferenceDatum;
    }

    public long Value { get; }

    public VerticalLimitUnit Unit { get; }

    public VerticalLimitReferenceDatum ReferenceDatum { get; }

}
