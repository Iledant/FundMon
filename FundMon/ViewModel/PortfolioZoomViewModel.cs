using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Repository;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public partial class PortfolioZoomViewModel : ObservableObject
{
    private readonly Portfolio _portfolio;

    [ObservableProperty]
    private ObservableCollection<MorningstarResponseLine> results;

    public ObservableCollection<FundPerformance> Performances;

    public PortfolioZoomViewModel(Portfolio seletecPortfolio)
    {
        _portfolio = seletecPortfolio;
        Performances = _portfolio.Funds;
    }

    public async Task<int> FetchMorningstarResults(string pattern)
    {
        Results = new ObservableCollection<MorningstarResponseLine>(await MorningStarHelpers.FetchFunds(pattern));
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

    public void RemoveFund(FundPerformance fund)
    {
        _portfolio.RemoveFund(fund);
    }
}
