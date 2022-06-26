using FundMon.Controls;
using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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

    private void GridView_DoubleTapped(object _1, DoubleTappedRoutedEventArgs _2)
    {
        if (GridView.SelectedItem is Portfolio portfolio)
            ViewModel.ShowFundsCommand.Execute(portfolio);
    }
}
