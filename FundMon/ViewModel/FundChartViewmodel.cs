using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FundMon.Controls;
using FundMon.Repository;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FundMon.ViewModel;

public partial class FundChartViewmodel : ObservableObject
{
    private FundPerformance _fund;

    [ObservableProperty]
    private List<DateValue> _values;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(HasNoAverage))]
    [AlsoNotifyChangeFor(nameof(Has8DaysAverage))]
    [AlsoNotifyChangeFor(nameof(Has15DaysAverage))]
    [AlsoNotifyChangeFor(nameof(Has30DaysAverage))]
    private int _averageCount;

    [ObservableProperty]
    private DateSelection _dateSelection = new(DateTime.MinValue,DateTime.MaxValue);

    [ObservableProperty]
    private bool _isZoomEnabled = false;

    public bool HasNoAverage => _averageCount == 0;

    public bool Has8DaysAverage => _averageCount == 8;

    public bool Has15DaysAverage => _averageCount == 15;

    public bool Has30DaysAverage => _averageCount == 30;

    public ICommand SetAverageCountCommand;
    public ICommand ZoomOutCommand;

    public FundPerformance Fund
    {
        get => _fund;
        set
        {
            _fund = value;
            Values = new(_fund.Fund.Historical);
        }
    }

    public FundChartViewmodel()
    {
        SetAverageCountCommand = new RelayCommand<int>(SetAverageCount);
        ZoomOutCommand = new RelayCommand(ZoomOut);
        AverageCount = 0;
    }

    private void SetAverageCount(int days)
    {
        AverageCount = days;
    }

    private void ZoomOut()
    {
        DateSelection = new(DateTime.MinValue,DateTime.MaxValue);
    }
}
