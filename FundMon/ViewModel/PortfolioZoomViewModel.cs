using FundMon.Repository;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FundMon.ViewModel;

public class PortfolioZoomViewModel : Bindable
{
    private Portfolio _portfolio;
    public ObservableCollection<FundPerformance> Performances;
    public List<MorningstarResponseLine> Results;

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

    public void AddFund(MorningstarResponseLine line, double averageCost)
    {
        Fund fund = Repo.AddFund(line.Name, line.MorningStarID);
        Repo.AddFundToPortfolio(_portfolio.ID, fund, averageCost);
    }

    public void UpdateAverageCost(int fundID, double averageCost)
    {
        Repo.UpdateFundAverageCost(_portfolio.ID, fundID, averageCost);
    }
}
