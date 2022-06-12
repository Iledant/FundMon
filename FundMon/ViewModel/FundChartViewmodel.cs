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
    private Dictionary<int, DateTime> _periods;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(HasNoAverage))]
    [AlsoNotifyChangeFor(nameof(Has8DaysAverage))]
    [AlsoNotifyChangeFor(nameof(Has15DaysAverage))]
    [AlsoNotifyChangeFor(nameof(Has30DaysAverage))]
    private int _averageCount;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(IsFullPeriod))]
    [AlsoNotifyChangeFor(nameof(Is1MonthPeriod))]
    [AlsoNotifyChangeFor(nameof(Is3MonthsPeriod))]
    [AlsoNotifyChangeFor(nameof(Is6MonthsPeriod))]
    [AlsoNotifyChangeFor(nameof(Is12MonthsPeriod))]
    private int _period = 0;

    [ObservableProperty]
    private DateSelection _dateSelection = new(DateTime.MinValue,DateTime.MaxValue);

    [ObservableProperty]
    private bool _isZoomEnabled = false;

    public bool HasNoAverage => _averageCount == 0;

    public bool Has8DaysAverage => _averageCount == 8;

    public bool Has15DaysAverage => _averageCount == 15;

    public bool Has30DaysAverage => _averageCount == 30;

    public bool IsFullPeriod => _period == 0;
    public bool Is1MonthPeriod => _period == 1;
    public bool Is3MonthsPeriod => _period == 2;
    public bool Is6MonthsPeriod => _period == 3;
    public bool Is12MonthsPeriod => _period == 4;

    public ICommand SetAverageCountCommand;
    public ICommand ZoomOutCommand;
    public ICommand SetPeriodCommand;

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
        SetPeriodCommand = new RelayCommand<int>(SetPeriod);
        AverageCount = 0;
        DateTime now = DateTime.Now;
        _periods = new Dictionary<int, DateTime> {
            { 0, DateTime.MinValue },
            { 1, now.AddMonths(-1) },
            { 2, now.AddMonths(-3) },
            { 3, now.AddMonths(-6) },
            { 4, now.AddMonths(-12) },
        };
    }

    private void SetAverageCount(int days)
    {
        AverageCount = days;
    }

    private void SetPeriod(int periodIndex)
    {
        if (_periods.ContainsKey(periodIndex))
        {
            DateTime periodBegin = _periods[periodIndex];
            DateSelection = new DateSelection(periodBegin, DateTime.Now);
            Period = periodIndex;
        }
    }

    private void ZoomOut()
    {
        DateSelection = new(DateTime.MinValue,DateTime.MaxValue);
    }
}
