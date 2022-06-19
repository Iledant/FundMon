using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FundMon.Controls;
using FundMon.Repository;
using Microsoft.UI.Xaml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FundMon.ViewModel;


// TODO: use ViewModel to navigate
public partial class PortfoliosViewModel : ObservableObject
{
    [ObservableProperty]
    private Portfolio editedPortfolio = new(0,"");

    [ObservableProperty]
    private ObservableCollection<Portfolio> portfolios = Repo.Portfolios;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(HasSelectedPortfolio))]
    private Portfolio selectedPortfolio = null;

    [ObservableProperty]
    private Visibility modalVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private string doneButtonName = "Créer";

    public bool HasSelectedPortfolio => selectedPortfolio is not null;

    public IRelayCommand<Portfolio> ShowEditPortfolioModalCommand { get; }

    public IRelayCommand<Portfolio> DeletePortfolioCommand { get; }

    public void EditModal_Done(object sender, DoneEventArgs e)
    {
        if (e.Escaped)
            return;
        if (EditedPortfolio.ID == 0)
            Repo.AddPortfolio(EditedPortfolio.Name, EditedPortfolio.Description);
        else
            Repo.UpdatePortfolio(EditedPortfolio.ID, EditedPortfolio.Name, EditedPortfolio.Description);
    }
    [ICommand]
    private void ShowAddPortfolioModal()
    {
        EditedPortfolio = new(0, "");
        DoneButtonName = "Ajouter";
        ModalVisibility = Visibility.Visible;
    }

    private void ShowEditPortfolioModal(Portfolio portfolio)
    {
        if (portfolio is null && SelectedPortfolio is null)
            return;
        EditedPortfolio = portfolio ?? SelectedPortfolio;
        DoneButtonName = "Modifier";
        ModalVisibility = Visibility.Visible;
    }

    public PortfoliosViewModel()
    {
        ShowEditPortfolioModalCommand = new RelayCommand<Portfolio>(ShowEditPortfolioModal);
        DeletePortfolioCommand = new RelayCommand<Portfolio>(DeletePortfolio);
    }

    private void DeletePortfolio(Portfolio portfolio)
    {
        if (portfolio is not null)
            Repo.RemovePortfolio(portfolio.ID);
    }
}
