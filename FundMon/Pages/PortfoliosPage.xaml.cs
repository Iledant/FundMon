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
            AddPortfolio();
        else
            ViewModel.UpdatePortfolio(SelectedPortfolio.ID, NameTextBox.Text, DescriptionTextBox.Text);
        AddEditGrid.Visibility = Visibility.Collapsed;
    }

    private void AddPortfolio()
    {
        ViewModel.AddPortfolio(NameTextBox.Text, DescriptionTextBox.Text);
        NameTextBox.Text = "";
        DescriptionTextBox.Text = "";

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
        if (GridView.SelectedItem is Portfolio portfolio && portfolio != SelectedPortfolio)
        {
            EditAppBarButton.IsEnabled = true;
            SelectedPortfolio = portfolio;
            AddEditGrid.Visibility = Visibility.Collapsed;
        }
        else
        {
            EditAppBarButton.IsEnabled = false;
            AddEditGrid.Visibility = Visibility.Collapsed;
            SelectedPortfolio = null;
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

    private void NameTextBoxAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        if (NameTextBox.Text != "")
            AddPortfolio();
    }

    private void EditAppBarButton_Click(object sender, RoutedEventArgs e)
    {
        if (GridView.SelectedItem is Portfolio portfolio)
        {
            SelectedPortfolio = portfolio;
            NameTextBox.Text = SelectedPortfolio.Name;
            DescriptionTextBox.Text = SelectedPortfolio.Description;
            AddButton.Content = "Modifier";
            AddEditGrid.Visibility = Visibility.Visible;
        }
    }

    private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedPortfolio = null;
        NameTextBox.Text = "";
        DescriptionTextBox.Text = "";
        AddButton.Content = "Ajouter";
        AddButton.IsEnabled = false;
        AddEditGrid.Visibility = Visibility.Visible;
    }
}
