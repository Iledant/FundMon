using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;

namespace FundMon.Controls;

public sealed partial class FundAddModal : UserControl
{
    private FundAddModalViewModel ViewModel;

    public delegate void DoneEventHandler(object sender, DoneEventArgs e);

    public event DoneEventHandler Done;
    public Visibility VisualState
    {
        get => (Visibility)GetValue(VisualStateProperty);
        set => SetValue(VisualStateProperty, value);
    }

    public static readonly DependencyProperty VisualStateProperty =
        DependencyProperty.Register(nameof(VisualState), typeof(Visibility), typeof(PortfolioEditModal), new PropertyMetadata(Visibility.Collapsed));

    public FundAddModal()
    {
        InitializeComponent();
        ViewModel = new();
    }

    private void RectangleTapped(object _1, TappedRoutedEventArgs _2)
    {
        Escape();
    }

    private void EscapeButton_Click(object _1, RoutedEventArgs _2)
    {
        Escape();
    }
    private void EscapeKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        Escape();
    }

    private void Escape()
    {
        VisualState = Visibility.Collapsed;
        Done?.Invoke(this, new DoneEventArgs(true));
    }

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

    private void DoneButton_Click(object sender, RoutedEventArgs e)
    {

    }
}
