using FundMon.Repository;
using System.Collections.Generic;

namespace FundMon.ViewModel;

public class RepositoryViewModel : Bindable
{
    private readonly Repo _repository = new();
    private List<Portfolio> _portfolios = new();
    private List<Fund> _funds = new();
    private List<Fund> _portfolioFunds = new();
    private List<FundPerformance> _portfolioFundsPerformance = new();
    private List<MorningstarResponseLine> _morningstarResults = new();

    public RepositoryViewModel(Repo repository)
    {
        _repository= repository;
        Portfolios = repository.Portfolios;
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
        _repository.AddPortfolio(name,description);
        Portfolios = _repository.Portfolios;
    }

    public void UpdatePortfolio(int portfolioID, string name, string description)
    {
        _repository.UpdatePortfolio(portfolioID, name,description);
        Portfolios = _repository.Portfolios;
    }

    public void RemovePortfolio(int portfolioID)
    {
        _repository.RemovePortfolio(portfolioID);
        Portfolios = _repository.Portfolios;
    }

    public void AddFund(string name, string description)
    {
        _repository.AddFund(name,description);
        Funds = _repository.Funds;
    }

    public void  UpdateFund(int fundID, string name, string description)
    {
        _repository.UpdateFund(fundID,name,description);
        Funds = _repository.Funds;
    }

    public void RemoveFund(int fundID)
    {
        _repository.RemoveFund(fundID);
        Funds = _repository.Funds;
    }

    public void FetchPortfolioFunds(Portfolio p)
    {
        PortfolioFunds = _repository.PortfolioFunds(p.ID);
    }

    public void FetchPortfolioFundsPerformance(Portfolio p)
    {
        PortfolioFundsPerformance = _repository.PortfolioPerformance(p.ID);
    }

    public async void FetchMorningstarResults(string pattern)
    {
        MorningstarResults = await MorningStarHelpers.FetchFunds(pattern);
    }
}
