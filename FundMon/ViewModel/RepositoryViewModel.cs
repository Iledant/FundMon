using FundMon.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public class RepositoryViewModel : Bindable
{
    private List<Portfolio> _portfolios = new();
    private List<Fund> _funds = new();
    private List<Fund> _portfolioFunds = new();
    private List<FundPerformance> _portfolioFundsPerformance = new();
    private List<MorningstarResponseLine> _morningstarResults = new();

    public RepositoryViewModel()
    {
        Portfolios = Repo.Portfolios;
    }

    public List<Portfolio> Portfolios
    {
        get => _portfolios;
        set
        {
            _portfolios = new List<Portfolio>(value);
            OnPropertyChanged(nameof(Portfolios));
        }
    }

    public List<Fund> Funds
    {
        get => _funds;
        set
        {
            _funds = new List<Fund>(value);
            OnPropertyChanged(nameof(Funds));
        }
    }

    public List<Fund> PortfolioFunds
    {
        get => _portfolioFunds;
        set
        {
            _portfolioFunds= new List<Fund>(value);
            OnPropertyChanged(nameof(PortfolioFunds));
        }
    }

    public List<FundPerformance> PortfolioFundsPerformance
    {
        get => _portfolioFundsPerformance;
        set
        {
            _portfolioFundsPerformance = new List<FundPerformance>(value);
            OnPropertyChanged(nameof(PortfolioFundsPerformance));
        }
    }

    public List<MorningstarResponseLine> MorningstarResults
    {
        get => _morningstarResults;
        set
        {
            _morningstarResults = new List<MorningstarResponseLine>(value);
            OnPropertyChanged(nameof(MorningstarResults));
        }
    }

    public void AddPortfolio(string name, string description)
    {
        Repo.AddPortfolio(name,description);
        Portfolios = Repo.Portfolios;
    }

    public void UpdatePortfolio(int portfolioID, string name, string description)
    {
        Repo.UpdatePortfolio(portfolioID, name,description);
        Portfolios = Repo.Portfolios;
    }

    public void RemovePortfolio(int portfolioID)
    {
        Repo.RemovePortfolio(portfolioID);
        Portfolios = Repo.Portfolios;
    }

    public void AddFund(string name, string description)
    {
        Repo.AddFund(name,description);
        Funds = Repo.Funds;
    }

    public void  UpdateFund(int fundID, string name, string description)
    {
        Repo.UpdateFund(fundID,name,description);
        Funds = Repo.Funds;
    }

    public void RemoveFund(int fundID)
    {
        Repo.RemoveFund(fundID);
        Funds = Repo.Funds;
    }

    public void FetchPortfolioFunds(Portfolio p)
    {
        PortfolioFunds = Repo.PortfolioFunds(p.ID);
    }

    public void FetchPortfolioFundsPerformance(Portfolio p)
    {
        PortfolioFundsPerformance = Repo.PortfolioPerformance(p.ID);
    }

    public async Task<int> FetchMorningstarResults(string pattern)
    {
        MorningstarResults = await MorningStarHelpers.FetchFunds(pattern);
        return MorningstarResults.Count;
    }
}
