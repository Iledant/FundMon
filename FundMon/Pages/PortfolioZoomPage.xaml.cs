using FundMon.Helpers;
using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FundMon.Pages
{
    public sealed partial class PortfolioZoomPage : Page
    {
        private Portfolio SelectedPortfolio = null;
        private PortfolioZoomViewModel ViewModel = null;
        private FundPerformance selectedFundPerformance = null;
        static CultureInfo ci = new("fr-FR");
        private double averageCost;
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

        private async void FundSearchButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FundSearchProgressRing.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            FundSearchButton.IsEnabled = false;
            await Task.Delay(1);
            int _ = await ViewModel.FetchMorningstarResults(FundSearchTextBox.Text);
            await Task.Delay(1);
            FundSearchProgressRing.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            FundSearchButton.IsEnabled = true;
        }

        private void FundSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FundSearchButton.IsEnabled = FundSearchTextBox.Text != "";
        }

        private void AddFundButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MorningstarResponseLine result = FundSearchGridView.SelectedItem as MorningstarResponseLine;
            ViewModel.AddFund(result, averageCost);
            AverageCostTextBox.IsEnabled = false;
            AverageCostTextBox.Text = "";
            FundSearchGridView.SelectedItem = null;
            AddFundButton.IsEnabled = false;
        }

        private void FundSearchGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool fundSelected = FundSearchGridView.SelectedItem != null;
            AverageCostTextBox.IsEnabled = fundSelected;
            AddFundStackPannel.Visibility = fundSelected ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        private void AverageCostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                averageCost = double.Parse(AverageCostTextBox.Text, ci);
                AddFundButton.IsEnabled = true;
            }
            catch (Exception)
            {
                AddFundButton.IsEnabled = false;
            }
        }

        private void BackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
            else
                this.Frame.Navigate(typeof(PortfoliosPage));
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedFundPerformance = FundDataGrid.SelectedItem is FundPerformance ? FundDataGrid.SelectedItem as FundPerformance : null;
            if (selectedFundPerformance is null)
            {
                SelectedFundStackPanel.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                SelectedFundStackPanel.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                averageCost = selectedFundPerformance.AverageCost;
                SelectedFundAverageCostTextBox.Text = averageCost.ToString("G", Formatter.Culture);
            }
        }

        private void SelectedFundDeleteButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void SelectedFundAverageCostTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                averageCost = double.Parse(SelectedFundAverageCostTextBox.Text, Formatter.Culture);
                SelectedFundAverageCostEditButton.IsEnabled = true;
            }
            catch (Exception)
            {
                SelectedFundAverageCostEditButton.IsEnabled = false;
            }
        }

        private void SelectedFundAverageCostEditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Repo.UpdateFundAverageCost(SelectedPortfolio.ID, selectedFundPerformance.Fund.ID, averageCost);
            //TODO find a way to update the datagrid content
        }
    }
}
