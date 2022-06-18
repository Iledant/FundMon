using FundMon.Controls;
using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace FundMon.Pages;

public sealed partial class PortfoliosPage : Page
{
    private readonly PortfoliosViewModel ViewModel;
    public PortfoliosPage()
    {
        InitializeComponent();
        ViewModel = new();
    }

    private void EditModal_Done(object sender, DoneEventArgs e)
    {
        if (e.Escaped)
            return;
        if (ViewModel.SelectedPortfolio.ID == 0)
            ViewModel.AddPortfolio();
        else
            ViewModel.UpdatePortfolio();
    }

    private void GridView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (GridView.SelectedItem is Portfolio portfolio)
            NavigateToPortfolioZoom(portfolio);
    }

    private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridView.SelectedItem is Portfolio portfolio && portfolio != ViewModel.SelectedPortfolio)
        {
            EditAppBarButton.IsEnabled = true;
            ViewModel.SelectedPortfolio = portfolio;
        }
        else
        {
            EditAppBarButton.IsEnabled = false;
        }
    }

    private void MenuFlyoutForward_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is Portfolio portfolio)
            NavigateToPortfolioZoom(portfolio);
    }

    private void NavigateToPortfolioZoom(Portfolio portfolio)
    {
        this.Frame.Navigate(typeof(PortfolioZoomPage), portfolio);
    }

    private void MenuFlyoutDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is Portfolio portfolio)
            ViewModel.Portfolios.Remove(portfolio);
    }

    private void EditAppBarButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedPortfolio is not null)
            ViewModel.ShowEditPortfolioModal();
    }

    private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ShowAddPortfolioModal();
    }

    private void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is Portfolio portfolio)
        {
            ViewModel.SelectedPortfolio = portfolio;
            ViewModel.ShowEditPortfolioModal();
        }
    }
}
