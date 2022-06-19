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

    private void GridView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (GridView.SelectedItem is Portfolio portfolio)
            NavigateToPortfolioZoom(portfolio);
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
}
