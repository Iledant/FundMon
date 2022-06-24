using CommunityToolkit.WinUI.UI.Controls;
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
    private readonly PortfolioZoomViewModel ViewModel;
    static readonly CultureInfo ci = new("fr-FR");
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

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
        else
            Frame.Navigate(typeof(PortfoliosPage));
    }

    private void AddFundAppBarButton_Click(object sender, RoutedEventArgs e) => Modal.Show();

    private void FundDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        string tag = e.Column.Tag.ToString();

        try
        {
            PortfolioZoomViewModel.ColumnTag columnTag = (PortfolioZoomViewModel.ColumnTag)Enum.Parse(typeof(PortfolioZoomViewModel.ColumnTag), tag);
            e.Column.SortDirection = ViewModel.SortFunds(columnTag);
            foreach (DataGridColumn column in FundDataGrid.Columns)
                if (column.Tag != e.Column.Tag)
                    column.SortDirection = null;
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
}
