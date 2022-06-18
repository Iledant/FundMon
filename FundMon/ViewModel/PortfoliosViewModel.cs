using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Repository;
using Microsoft.UI.Xaml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FundMon.ViewModel;

public partial class PortfoliosViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Portfolio> portfolios = Repo.Portfolios;

    [ObservableProperty]
    private Portfolio selectedPortfolio = new(0,"");

    [ObservableProperty]
    private Visibility modalVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private string doneButtonName = "Créer";

    public void AddPortfolio()
    {
        Repo.AddPortfolio(SelectedPortfolio.Name, SelectedPortfolio.Description);
    }

    public void UpdatePortfolio()
    {
        Repo.UpdatePortfolio(SelectedPortfolio.ID, SelectedPortfolio.Name, SelectedPortfolio.Description);
    }

    public void ShowAddPortfolioModal()
    {
        SelectedPortfolio = new(0, "");
        DoneButtonName = "Ajouter";
        ModalVisibility = Visibility.Visible;
    }

    public void ShowEditPortfolioModal()
    {
        DoneButtonName = "Modifier";
        ModalVisibility = Visibility.Visible;
    }
}
