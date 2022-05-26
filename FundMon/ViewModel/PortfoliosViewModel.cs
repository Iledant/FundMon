using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Repository;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FundMon.ViewModel;

public partial class PortfoliosViewModel : ObservableObject
{
    private readonly Dictionary<string, List<string>> _propertyNameToErrorsDictionary = new();

    [ObservableProperty]
    private ObservableCollection<Portfolio> portfolios = Repo.Portfolios;

    public bool HasErrors => _propertyNameToErrorsDictionary.Any();

    public void AddPortfolio(string name, string description)
    {
        Repo.AddPortfolio(name, description);
    }

    public IEnumerable GetErrors(string propertyName)
    {
        return _propertyNameToErrorsDictionary.GetValueOrDefault(propertyName, new List<string>());
    }

    public void UpdatePortfolio(int portfolioID, string name, string description)
    {
        Repo.UpdatePortfolio(portfolioID, name, description);
    }
}
