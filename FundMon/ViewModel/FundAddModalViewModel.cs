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
    public delegate void SetVisualState(Visibility v);

    private readonly SetVisualState setVisualState;

    public Portfolio SelectedPortfolio;

    private double averageCost = 0;

    [ObservableProperty]
    private List<MorningstarResponseLine> results;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(HasValidValues))]
    private MorningstarResponseLine selectedLine;

    [ObservableProperty]
    private bool iSInProgress = false;

    public bool HasValidValues => selectedLine is not null && averageCost > 0;

    public FundAddModalViewModel(SetVisualState setter)
    {
        setVisualState = setter;
    }

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
        if (!double.TryParse(text.Replace('.',','), out averageCost))
            averageCost = 0;
        OnPropertyChanged(nameof(HasValidValues));
    }

    [ICommand]
    public async void FundAdd()
    {
        if (!HasValidValues || SelectedPortfolio is null)
            return;
        ISInProgress = true;
        await Task.Delay(1);
        Fund fund = await Repo.AddFund(selectedLine.Name, selectedLine.MorningStarID);
        Repo.AddFundToPortfolio(SelectedPortfolio.ID, fund, averageCost);
        ISInProgress = false;
        await Task.Delay(1);
        setVisualState?.Invoke(Visibility.Collapsed);
    }
}
