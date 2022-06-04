using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Repository;
using System.Collections.Generic;

namespace FundMon.ViewModel;

public partial class FundChartViewmodel : ObservableObject
{
    private FundPerformance _fund;
    
    [ObservableProperty]
    private List<DateValue> _values;

    [ObservableProperty]
    private bool _averageValuesIsOn = false;

    [ObservableProperty]
    private int _averageCount = 8;

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

    }
}
