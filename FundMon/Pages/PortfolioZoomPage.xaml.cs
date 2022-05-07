using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

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
        private PortfolioZoomViewModel ViewModel = null;
        public PortfolioZoomPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Portfolio)
            {
                SelectedPortfolio = e.Parameter as Portfolio;
                TitleTextBox.Text = "Portefeuille " + SelectedPortfolio.Name;
                ViewModel = new(SelectedPortfolio);
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

        private async void FundSearchButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FundSearchProgressBar.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            await Task.Delay(1);
            int _  = await ViewModel.FetchMorningstarResults(FundSearchTextBox.Text);
            await Task.Delay(1);
            FundSearchProgressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        private void FundSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FundSearchButton.IsEnabled = FundSearchTextBox.Text != "";
        }

        private void AddFundButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MorningstarResponseLine result = FundSearchGridView.SelectedItem as MorningstarResponseLine;
            ViewModel.AddFund(result);
        }

        private void FundSearchGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddFundButton.IsEnabled = FundSearchGridView.SelectedItem != null;
        }
    }
}
