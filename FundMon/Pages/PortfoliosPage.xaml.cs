using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FundMon.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PortfoliosPage : Page
{
    private RepositoryViewModel ViewModel;
    private Portfolio SelectedPortfolio = null;
    private Repo Repo = null;
    public PortfoliosPage()
    {
        this.InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is Repo)
        {
            Repo = (Repo)e.Parameter;
            ViewModel = new(Repo);
        }
        else
        {
            throw new Exception("Repo parameter expected");
        }
        
        base.OnNavigatedTo(e);
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
        if (GridView.SelectedItem is Portfolio)
        {
            this.Frame.Navigate(typeof(PortfolioZoomPage),
                new RepoAndSelectedPortfolio{ 
                    SelectedPortfolio = GridView.SelectedItem as Portfolio, 
                    Repo = Repo });
        }
    }

    private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridView.SelectedItem is Portfolio)
        {
            SelectedPortfolio= (Portfolio)GridView.SelectedItem;
            NameTextBox.Text = SelectedPortfolio.Name;
            DescriptionTextBox.Text =SelectedPortfolio.Description;
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
}
