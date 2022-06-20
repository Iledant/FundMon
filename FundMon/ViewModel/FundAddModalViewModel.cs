using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public partial class FundAddModalViewModel : ObservableObject
{
    private double averageCost = 0;

    [ObservableProperty]
    private List<MorningstarResponseLine> results;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(HasValidValues))]
    private MorningstarResponseLine selectedLine;

    [ObservableProperty]
    private bool iSInProgress = false;

    public bool HasValidValues => selectedLine is not null && averageCost > 0;

    public async void FundSearch(string pattern)
    {
        ISInProgress = true;
        await Task.Delay(1);
        Results = await MorningStarHelpers.FetchFunds(pattern);
        ISInProgress = false;
        await Task.Delay(1);
    }

    public void ParseAverageCostText(string text)
    {
        if (!double.TryParse(text, out averageCost))
            averageCost = 0;
        OnPropertyChanged(nameof(HasValidValues));
    }
}
