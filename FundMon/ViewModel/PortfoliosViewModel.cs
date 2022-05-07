using FundMon.Repository;
using System.Collections.Generic;

namespace FundMon.ViewModel;

public class PortfoliosViewModel : Bindable
{
    private List<Portfolio> _portfolios;

    public List<Portfolio> Portfolios
    {
        get => _portfolios;
        set
        {
            _portfolios = value;
            OnPropertyChanged();
        }
    }

    public PortfoliosViewModel()
    {
        Portfolios = new List<Portfolio>(Repo.Portfolios);
    }
    public void AddPortfolio(string name, string description)
    {
        Repo.AddPortfolio(name, description);
        Portfolios = new List<Portfolio>(Repo.Portfolios);
    }

    public void UpdatePortfolio(int portfolioID, string name, string description)
    {
        Repo.UpdatePortfolio(portfolioID, name, description);
        Portfolios = new List<Portfolio>(Repo.Portfolios);
    }


}
