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
            }
            else
            {
                throw new Exception("Portfolio parameters expected");
            }

            base.OnNavigatedTo(e);
        }

        private void GridView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {

        }

        private void FundGridView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {

        }
    }
}
