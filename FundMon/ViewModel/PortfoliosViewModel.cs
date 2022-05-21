using FundMon.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FundMon.ViewModel;

public class PortfoliosViewModel : Bindable
{
    public ObservableCollection<Portfolio> Portfolios = Repo.Portfolios;

    public void AddPortfolio(string name, string description)
    {
        Repo.AddPortfolio(name, description);
    }

    public void UpdatePortfolio(int portfolioID, string name, string description)
    {
        Repo.UpdatePortfolio(portfolioID, name, description);
    }


}
