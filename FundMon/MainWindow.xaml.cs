using FundMon.Pages;
using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FundMon;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly Repo Repository;

    public MainWindow()
    {
        InitializeComponent();
        Repository = new Repo();
        RootFrame.Navigate(typeof(PortfoliosPage), Repository);
    }
}
