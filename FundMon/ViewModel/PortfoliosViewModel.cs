using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Repository;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FundMon.ViewModel;

public partial class PortfoliosViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Portfolio> portfolios = Repo.Portfolios;

    public void AddPortfolio(string name, string description)
    {
        Repo.AddPortfolio(name, description);
    }

    public void UpdatePortfolio(int portfolioID, string name, string description)
    {
        Repo.UpdatePortfolio(portfolioID, name, description);
    }
}
