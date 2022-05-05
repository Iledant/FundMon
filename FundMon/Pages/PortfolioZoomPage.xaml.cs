using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FundMon.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PortfolioZoomPage : Page
    {
        private Portfolio SelectedPortfolio = null;
        private Repo Repo = null;
        private RepositoryViewModel ViewModel = null;
        public PortfolioZoomPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is RepoAndSelectedPortfolio)
            {
                (SelectedPortfolio, Repo) = e.Parameter as RepoAndSelectedPortfolio;
                TitleTextBox.Text = "Portefeuille " + SelectedPortfolio.Name;
                ViewModel = new(Repo);
                ViewModel.FetchPortfolioFunds(SelectedPortfolio);
            }
            else
            {
                throw new Exception("Portfolio parameters expected");
            }

            base.OnNavigatedTo(e);
        }

        private void FundGridView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {

        }

        private void FundGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FundSearchButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.FetchMorningstarResults(FundSearchTextBox.Text);
        }

        private void FundSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FundSearchButton.IsEnabled = FundSearchTextBox.Text != "";
        }

        private void AddFundButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MorningstarResponseLine result = FundSearchGridView.SelectedItem as MorningstarResponseLine;
            int fundID = Repo.AddFund(result.Name, result.MorningStarID);
            Repo.AddFundToPortfolio(SelectedPortfolio.ID, fundID, 0);
        }

        private void FundSearchGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddFundButton.IsEnabled = FundSearchGridView.SelectedItem != null;
        }
    }
}
