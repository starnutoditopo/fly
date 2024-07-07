using Fly.Models.UnitsOfMeasure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Fly.ViewModels;

public class ConversionsViewModel : ViewModelBase
{
    public ConversionsViewModel()
    {
        SourceUnitsOfMeasure = new ObservableCollection<IUnitOfMeasure>() {
            UnitsOfMeasure.Kilometer,
            UnitsOfMeasure.Mile,
            UnitsOfMeasure.LitersPerHour,
            UnitsOfMeasure.LitersPerSecond,
            UnitsOfMeasure.GallonsPerHour,
            UnitsOfMeasure.GallonsPerSecond,
            UnitsOfMeasure.Knot,
            UnitsOfMeasure.KilometerPerHour,
            UnitsOfMeasure.Meter,
            UnitsOfMeasure.Foot,
            UnitsOfMeasure.MeterPerSecond,
            UnitsOfMeasure.Gallon,
            UnitsOfMeasure.Liter
        };

        TargetUnitsOfMeasure = new ObservableCollection<IUnitOfMeasure>();

        PropertyChanged += ConversionsViewModel_PropertyChanged;

        SourceUnitOfMeasure = SourceUnitsOfMeasure.FirstOrDefault();
        TargetUnitOfMeasure = TargetUnitsOfMeasure.FirstOrDefault();
    }

    private static IEnumerable<Type> GetUomInterfacesOf(Type t)
    {
        var interfaces = t
                   .GetInterfaces()
                   .Where(x =>
                       x.IsGenericType &&
                       x.GetGenericTypeDefinition() == typeof(IUnitOfMeasure<>));
        return interfaces;
    }

    private void ConversionsViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SourceUnitOfMeasure))
        {
            TargetUnitOfMeasure = null;
            Output = null;
            TargetUnitsOfMeasure.Clear();

            if (SourceUnitOfMeasure != null)
            {
                Type t = SourceUnitOfMeasure.GetType();
                var interfaces = GetUomInterfacesOf(t).ToList();
                foreach (var availableUom in SourceUnitsOfMeasure)
                {
                    if (availableUom != SourceUnitOfMeasure)
                    {
                        foreach (var uom in interfaces)
                        {
                            if (availableUom.GetType().IsAssignableTo(uom))
                            {
                                TargetUnitsOfMeasure.Add(availableUom);
                            }
                        }
                    }
                }
                TargetUnitOfMeasure = TargetUnitsOfMeasure.FirstOrDefault();
            }
        }

        if (
            (e.PropertyName == nameof(SourceUnitOfMeasure))
            || (e.PropertyName == nameof(TargetUnitOfMeasure))
            || (e.PropertyName == nameof(Input))
        )
        {
            UpdateOutput();
        }
    }

    private double? _input;
    public virtual double? Input
    {
        get => _input;
        set => SetProperty(ref _input, value);
    }

    private double? _output;
    public virtual double? Output
    {
        get => _output;
        set => SetProperty(ref _output, value);
    }

    private object? _sourceUnitOfMeasure;
    public virtual object? SourceUnitOfMeasure
    {
        get => _sourceUnitOfMeasure;
        set => SetProperty(ref _sourceUnitOfMeasure, value);
    }

    private object? _targetUnitOfMeasure;
    public virtual object? TargetUnitOfMeasure
    {
        get => _targetUnitOfMeasure;
        set => SetProperty(ref _targetUnitOfMeasure, value);
    }


    public ObservableCollection<IUnitOfMeasure> SourceUnitsOfMeasure { get; }
    public ObservableCollection<IUnitOfMeasure> TargetUnitsOfMeasure { get; }

    private void UpdateOutput()
    {
        if (SourceUnitOfMeasure != null)
        {
            if (TargetUnitOfMeasure != null)
            {
                if (Input != null)
                {
                    var valueInStdUom = GetUomInterfacesOf(SourceUnitOfMeasure.GetType())
                        .FirstOrDefault()
                        .GetMethod(nameof(IUnitOfMeasure<IQuantity>.ToStandardUnits))
                        .Invoke(SourceUnitOfMeasure, [Input]);

                    var result = GetUomInterfacesOf(TargetUnitOfMeasure.GetType())
                        .FirstOrDefault()
                        .GetMethod(nameof(IUnitOfMeasure<IQuantity>.FromStandardUnits))
                        .Invoke(TargetUnitOfMeasure, [valueInStdUom]);



                    Output = (double?)result;
                    return;
                }
            }
        }
        Output = null;
    }

    public bool CanSwitchUnitsOfMeasure => true;

    public async Task SwitchUnitsOfMeasure()
    {
        var originalSource = SourceUnitOfMeasure;

        SourceUnitOfMeasure = TargetUnitOfMeasure;
        TargetUnitOfMeasure = originalSource;

        await Task.CompletedTask;
    }
}