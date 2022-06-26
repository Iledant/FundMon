using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using FundMon.Pages;
using FundMon.Repository;
using FundMon.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public partial class PortfolioZoomViewModel : ObservableObject
{
    private readonly INavigationService navigationService;
    private Portfolio _portfolio;
    private ColumnTag _previousSortedColumTag = ColumnTag.None;
    private DataGridSortDirection? _previousSortDirection = null;

    [ObservableProperty]
    private ObservableCollection<MorningstarResponseLine> results;

    [ObservableProperty]
    public ObservableCollection<FundPerformance> performances;

    public IRelayCommand<FundPerformance> DeleteFundCommand { get; }

    public IRelayCommand<FundPerformance> ShowChartCommand { get; }

    public IRelayCommand GoBackCommand { get; }

    public Portfolio Portfolio
    {
        get => _portfolio;
        set
        {
            _portfolio = value;
            OnPropertyChanged(nameof(Portfolio));
            Performances = _portfolio.Funds;
        }
    }

    public PortfolioZoomViewModel()
    {
        DeleteFundCommand = new RelayCommand<FundPerformance>(RemoveFund);
        ShowChartCommand = new RelayCommand<FundPerformance>(ShowChart);
        navigationService = App.Current.Services.GetService<INavigationService>();
        GoBackCommand = new RelayCommand(GoBack);
    }

    public enum ColumnTag { None = 0, AverageCost = 1, Evolution = 2, LastValue = 3, LastWeekValue = 4, LastMonthValue = 5 };

    public async Task<int> FetchMorningstarResults(string pattern)
    {
        List<MorningstarResponseLine> funds = await MorningStarHelpers.FetchFunds(pattern);
        Results = new ObservableCollection<MorningstarResponseLine>(funds);
        return Results.Count;
    }

    public async void AddFund(MorningstarResponseLine line, double averageCost)
    {
        Fund fund = await Repo.AddFund(line.Name, line.MorningStarID);
        Repo.AddFundToPortfolio(_portfolio.ID, fund, averageCost);
    }

    public void UpdateAverageCost(int fundID, double averageCost)
    {
        Repo.UpdateFundAverageCost(_portfolio.ID, fundID, averageCost);
    }

    private void RemoveFund(FundPerformance fund)
    {
        _portfolio.RemoveFund(fund);
    }

    public DataGridSortDirection? SortFunds(string tag)
    {
        ColumnTag columnTag;
        try
        {
            columnTag = (ColumnTag)Enum.Parse(typeof(ColumnTag), tag);
        }
        catch (Exception)
        {
            return null;
        }

        if (_previousSortedColumTag != columnTag)
        {
            _previousSortedColumTag = columnTag;
            _previousSortDirection = DataGridSortDirection.Ascending;
        }
        else
        {
            if (_previousSortDirection == DataGridSortDirection.Ascending)
                _previousSortDirection = DataGridSortDirection.Descending;
            else
                _previousSortDirection = DataGridSortDirection.Ascending;
        }

        IOrderedEnumerable<FundPerformance> orderedFunds = columnTag switch
        {
            ColumnTag.AverageCost => _previousSortDirection == DataGridSortDirection.Ascending ?
                    (from item in _portfolio.Funds orderby item.AverageCost ascending select item) :
                    (from item in _portfolio.Funds orderby item.AverageCost descending select item),
            ColumnTag.Evolution => (_previousSortDirection == DataGridSortDirection.Ascending) ?
                    (from item in _portfolio.Funds orderby item.Evolution ascending select item) :
                    (from item in _portfolio.Funds orderby item.Evolution descending select item),
            ColumnTag.LastValue => (_previousSortDirection == DataGridSortDirection.Ascending) ?
                   (from item in _portfolio.Funds orderby item.LastValue ascending select item) :
                   (from item in _portfolio.Funds orderby item.LastValue descending select item),
            ColumnTag.LastWeekValue => (_previousSortDirection == DataGridSortDirection.Ascending) ?
                    (from item in _portfolio.Funds orderby item.LastWeekValue ascending select item) :
                    (from item in _portfolio.Funds orderby item.LastWeekValue descending select item),
            ColumnTag.LastMonthValue => (_previousSortDirection == DataGridSortDirection.Ascending) ?
                     (from item in _portfolio.Funds orderby item.LastMonthValue ascending select item) :
                     (from item in _portfolio.Funds orderby item.LastMonthValue descending select item),
            _ => throw new System.NotImplementedException()
        };

        Performances = new(orderedFunds);
        return _previousSortDirection;
    }

    private void ShowChart(FundPerformance fundPerformance)
    {
        navigationService.Navigate(typeof(FundChart), fundPerformance);
    }

    private void GoBack() => navigationService.GoBack();
}
