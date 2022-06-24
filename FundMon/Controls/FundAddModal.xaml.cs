using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;

namespace FundMon.Controls;

public sealed partial class FundAddModal : UserControl
{
    private readonly FundAddModalViewModel ViewModel;

    public Portfolio SelectedPortfolio
    {
        get { return (Portfolio)GetValue(SelectedPortfolioProperty); }
        set
        {
            SetValue(SelectedPortfolioProperty, value);
            ViewModel.SelectedPortfolio = value;
        }
    }

    public static readonly DependencyProperty SelectedPortfolioProperty =
        DependencyProperty.Register(nameof(SelectedPortfolio), typeof(Portfolio), typeof(FundAddModal), new PropertyMetadata(null));

    public FundAddModal()
    {
        InitializeComponent();
        ViewModel = new();
    }

    private void RectangleTapped(object _1, TappedRoutedEventArgs _2) => Escape();

    private void EscapeButton_Click(object _1, RoutedEventArgs _2) => Escape();

    private void EscapeKeyboardAccelerator_Invoked(KeyboardAccelerator _1, KeyboardAcceleratorInvokedEventArgs _2) => Escape();

    private void Escape() => ViewModel.Visibility = Visibility.Collapsed;

    private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        async Task<bool> UserKeepsTyping()
        {
            string text = SearchTextBox.Text;
            await Task.Delay(500);
            return text != SearchTextBox.Text;
        }

        if (await UserKeepsTyping())
            return;
        ViewModel.FundSearch(SearchTextBox.Text);
    }

    private void AverageCostTextBox_TextChanged(object sender, TextChangedEventArgs e) => ViewModel.ParseAverageCostText(AverageCostTextBox.Text);

    public void Show() => ViewModel.Visibility = Visibility.Visible;
}
