using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FundMon.Repository;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public partial class FundAddModalViewModel : ObservableObject
{
    public Portfolio SelectedPortfolio;

    private double averageCost = 0;

    [ObservableProperty]
    private List<MorningstarResponseLine> results;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(HasValidValues))]
    private MorningstarResponseLine selectedLine;

    [ObservableProperty]
    private bool isInProgress = false;

    [ObservableProperty]
    private Visibility visibility = Visibility.Collapsed;

    public bool HasValidValues => selectedLine is not null && averageCost > 0;

    public async void FundSearch(string pattern)
    {
        IsInProgress = true;
        await Task.Delay(1);
        Results = await MorningStarHelpers.FetchFunds(pattern);
        IsInProgress = false;
        await Task.Delay(1);
    }

    [ICommand]
    public void ParseAverageCostText(string text)
    {
        if (!double.TryParse(text.Replace('.',','), out averageCost))
            averageCost = 0;
        OnPropertyChanged(nameof(HasValidValues));
    }

    [ICommand]
    public async void FundAdd()
    {
        if (!HasValidValues || SelectedPortfolio is null)
            return;
        IsInProgress = true;
        await Task.Delay(1);
        Fund fund = await Repo.AddFund(selectedLine.Name, selectedLine.MorningStarID);
        Repo.AddFundToPortfolio(SelectedPortfolio.ID, fund, averageCost);
        IsInProgress = false;
        await Task.Delay(1);
        Visibility=Visibility.Collapsed;
    }
}
