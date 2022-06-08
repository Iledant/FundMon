using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace FundMon.Pages;

public sealed partial class PortfolioZoomPage : Page
{
    private PortfolioZoomViewModel ViewModel;
    static readonly CultureInfo ci = new("fr-FR");
    private double averageCost;
    public PortfolioZoomPage()
    {
        InitializeComponent();
        ViewModel = new();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is Portfolio selectedPortfolio)
        {
            ViewModel.Portfolio = selectedPortfolio;
            TitleTextBox.Text = "Portefeuille " + ViewModel.Portfolio.Name;
        }
        else
            throw new Exception("Portfolio navigation parameter expected");

        base.OnNavigatedTo(e);
    }

    private async void FundSearchButton_Click(object sender, RoutedEventArgs e)
    {
        FundSearchProgressRing.Visibility = Visibility.Visible;
        FundSearchButton.IsEnabled = false;
        await Task.Delay(1);
        FundSearchGridView.Visibility = Visibility.Visible;
        StepTwoGrid.Visibility = Visibility.Visible;
        int _ = await ViewModel.FetchMorningstarResults(FundSearchTextBox.Text);
        await Task.Delay(1);
        FundSearchProgressRing.Visibility = Visibility.Collapsed;
        FundSearchButton.IsEnabled = true;
    }

    private void FundSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        FundSearchButton.IsEnabled = FundSearchTextBox.Text != "";
    }

    private void AddFundButton_Click(object sender, RoutedEventArgs e)
    {
        MorningstarResponseLine result = FundSearchGridView.SelectedItem as MorningstarResponseLine;
        ViewModel.AddFund(result, averageCost);
        ResetFundSearchGrid();
    }

    private void ResetFundSearchGrid()
    {
        AverageCostTextBox.Text = "";
        FundSearchGridView.SelectedItem = null;
        FundSearchGrid.Visibility = Visibility.Collapsed;
        FundSearchGridView.Visibility = Visibility.Collapsed;
        StepTwoGrid.Visibility = Visibility.Collapsed;
        StepThreeGrid.Visibility = Visibility.Collapsed;
        AverageCostTextBox.Visibility = Visibility.Collapsed;
    }

    private void FundSearchGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        bool fundSelected = FundSearchGridView.SelectedItem != null;
        AverageCostTextBox.Visibility = fundSelected ? Visibility.Visible : Visibility.Collapsed;
        StepThreeGrid.Visibility = fundSelected ? Visibility.Visible : Visibility.Collapsed;
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

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
        else
            Frame.Navigate(typeof(PortfoliosPage));
    }

    private void SearchAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        if (FundSearchTextBox.Text != "")
            FundSearchButton_Click(sender, null);
    }

    private void AddFundAppBarButton_Click(object sender, RoutedEventArgs e)
    {
        FundSearchGrid.Visibility = Visibility.Visible;
    }

    private void FundDataGrid_Sorting(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridColumnEventArgs e)
    {
        string tag = e.Column.Tag.ToString();

        try
        {
            PortfolioZoomViewModel.ColumnTag columnTag = (PortfolioZoomViewModel.ColumnTag)Enum.Parse(typeof(PortfolioZoomViewModel.ColumnTag), tag);
            e.Column.SortDirection = ViewModel.SortFunds(columnTag);
            foreach (CommunityToolkit.WinUI.UI.Controls.DataGridColumn column in FundDataGrid.Columns)
            {
                if (column.Tag != e.Column.Tag)
                    column.SortDirection = null;
            }
        }
        catch (Exception)
        { }
    }

    private void RowDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is FundPerformance fund)
            ViewModel.RemoveFund(fund);
    }

    private void RowShowChart_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is FundPerformance fund)
            Frame.Navigate(typeof(FundChart), fund);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ResetFundSearchGrid();
    }
}
