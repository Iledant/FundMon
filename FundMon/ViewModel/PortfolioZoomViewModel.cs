using FundMon.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public class PortfolioZoomViewModel : Bindable
{
    private readonly Portfolio _portfolio;
    private List<MorningstarResponseLine> results;
    public ObservableCollection<FundPerformance> Performances;

    public List<MorningstarResponseLine> Results
    {
        get => results;
        set {
            results = value;
            OnPropertyChanged();
        }
    }

    public PortfolioZoomViewModel(Portfolio seletecPortfolio)
    {
        _portfolio = seletecPortfolio;
        Performances = _portfolio.Funds;
    }

    public async Task<int> FetchMorningstarResults(string pattern)
    {
        Results = await MorningStarHelpers.FetchFunds(pattern);
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
}
