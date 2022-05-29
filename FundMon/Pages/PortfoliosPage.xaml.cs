using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace FundMon.Pages;

public sealed partial class PortfoliosPage : Page
{
    private readonly PortfoliosViewModel ViewModel;
    private Portfolio SelectedPortfolio = null;
    public PortfoliosPage()
    {
        InitializeComponent();
        ViewModel = new();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (NameTextBox.Text == "")
            return;
        if (AddButton.Content as string == "Ajouter")
        {
            ViewModel.AddPortfolio(NameTextBox.Text, DescriptionTextBox.Text);
            NameTextBox.Text = "";
            DescriptionTextBox.Text = "";
        }
        else
        {
            ViewModel.UpdatePortfolio(SelectedPortfolio.ID, NameTextBox.Text, DescriptionTextBox.Text);
        }
    }

    private void NameTextBox_TextChanged(object sender, Microsoft.UI.Xaml.Controls.TextChangedEventArgs e)
    {
        AddButton.IsEnabled = NameTextBox.Text != "";
    }

    private void GridView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (GridView.SelectedItem is Portfolio portfolio)
            NavigateToPortfolioZoom(portfolio);
    }

    private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridView.SelectedItem is Portfolio portfolio)
        {
            SelectedPortfolio = portfolio;
            NameTextBox.Text = SelectedPortfolio.Name;
            DescriptionTextBox.Text = SelectedPortfolio.Description;
            AddButton.Content = "Modifier";
        }
        else
        {
            SelectedPortfolio = null;
            NameTextBox.Text = "";
            DescriptionTextBox.Text = "";
            AddButton.Content = "Ajouter";
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
}
