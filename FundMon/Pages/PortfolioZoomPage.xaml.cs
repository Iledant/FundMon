using FundMon.Helpers;
using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FundMon.Pages;

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

    private void MenuFlyoutDeleteItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is FundPerformance fund)
            ViewModel.RemoveFund(fund);
    }

    private void SearchAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        if (FundSearchTextBox.Text != "")
            FundSearchButton_Click(sender, null);
    }
}
