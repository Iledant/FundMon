using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FundMon.Controls;
using FundMon.Pages;
using FundMon.Repository;
using FundMon.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FundMon.ViewModel;

public partial class PortfoliosViewModel : ObservableObject
{
    private readonly INavigationService navigationService;

    [ObservableProperty]
    private Portfolio editedPortfolio = new(0, "");

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

    public IRelayCommand<Portfolio> ShowFundsCommand { get; }

    public PortfoliosViewModel()
    {
        ShowEditPortfolioModalCommand = new RelayCommand<Portfolio>(ShowEditPortfolioModal);
        DeletePortfolioCommand = new RelayCommand<Portfolio>(DeletePortfolio);
        ShowFundsCommand = new RelayCommand<Portfolio>(ShowFunds);
        navigationService = App.Current.Services.GetService<INavigationService>();
    }

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

    private void DeletePortfolio(Portfolio portfolio)
    {
        if (portfolio is not null)
            Repo.RemovePortfolio(portfolio.ID);
    }

    private void ShowFunds(Portfolio porfolio)
    {
        navigationService.Navigate(typeof(PortfolioZoomPage), porfolio);
    }
}
