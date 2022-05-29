﻿using FundMon.Repository;
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
    private Portfolio SelectedPortfolio = null;
    private PortfolioZoomViewModel ViewModel = null;
    static readonly CultureInfo ci = new("fr-FR");
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
        AverageCostTextBox.Text = "";
        FundSearchGridView.SelectedItem = null;
        AddFundButton.IsEnabled = false;
        FundSearchGrid.Visibility = Visibility.Collapsed;
        FundSearchGridView.Visibility = Visibility.Collapsed;
        StepTwoGrid.Visibility=Visibility.Collapsed;
        StepThreeGrid.Visibility = Visibility.Collapsed;
        AddFundStackPannel.Visibility = Visibility.Collapsed;
    }

    private void FundSearchGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        bool fundSelected = FundSearchGridView.SelectedItem != null;
        AverageCostTextBox.IsEnabled = fundSelected;
        AddFundStackPannel.Visibility = fundSelected ? Visibility.Visible : Visibility.Collapsed;
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
        if (this.Frame.CanGoBack)
            this.Frame.GoBack();
        else
            this.Frame.Navigate(typeof(PortfoliosPage));
    }

    private void MenuFlyoutDeleteItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.DataContext is FundPerformance fund)
            ViewModel.RemoveFund(fund);
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
}
