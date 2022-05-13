using FundMon.Repository;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FundMon.ViewModel;

public class PortfolioZoomViewModel : Bindable
{
    private Portfolio _portfolio;
    private List<FundPerformance> _performances;
    private List<MorningstarResponseLine> _results;

    public List<FundPerformance> Performances
    {
        get => _performances;
        set
        {
            _performances = value;
            OnPropertyChanged();
        }
    }

    public List<MorningstarResponseLine> Results
    {
        get => _results;
        set
        {
            _results = value;
            OnPropertyChanged();
        }
    }

    public PortfolioZoomViewModel(Portfolio seletecPortfolio)
    {
        _portfolio = seletecPortfolio;
        Performances = Repo.PortfolioPerformance(_portfolio.ID);
    }

    public async Task<int> FetchMorningstarResults(string pattern)
    {
        Results = await MorningStarHelpers.FetchFunds(pattern);
        return Results.Count;
    }

    public void AddFund(MorningstarResponseLine line, double averageCost)
    {
        int fundID = Repo.AddFund(line.Name, line.MorningStarID);
        Repo.AddFundToPortfolio(_portfolio.ID, fundID, averageCost);
        Performances = Repo.PortfolioPerformance(_portfolio.ID);
    }
}
