using FundMon.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace FundMon.Pages;

/// <summary>
/// The page used to display all logs
/// </summary>
public sealed partial class LogPage : Page
{
    private LogViewModel ViewModel;

    public LogPage()
    {
        InitializeComponent();
        ViewModel = new();
    }
}
