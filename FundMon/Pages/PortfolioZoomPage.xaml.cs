using CommunityToolkit.WinUI.UI.Controls;
using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace FundMon.Pages;

public sealed partial class PortfolioZoomPage : Page
{
    private readonly PortfolioZoomViewModel ViewModel;
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

    private void AddFundAppBarButton_Click(object sender, RoutedEventArgs e) => Modal.Show();

    private void FundDataGrid_Sorting(object sender, DataGridColumnEventArgs e)
    {
        string tag = e.Column.Tag.ToString();
        e.Column.SortDirection = ViewModel.SortFunds(tag);
        foreach (DataGridColumn column in FundDataGrid.Columns)
            if (column.Tag != e.Column.Tag)
                column.SortDirection = null;
    }
}
