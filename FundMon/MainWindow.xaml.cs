using FundMon.Pages;
using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace FundMon;

public sealed partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel;

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);
        Activated += MainWindow_Activated;

        Repo.Load(Config.Config.File);
        RootFrame.Navigate(typeof(PortfoliosPage));
        Repo.UpdateFundsHistorical();
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        Repo.Save(Config.Config.File);
        Config.Config.SaveAndClose();
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
            AppTitleTextBlock.Foreground =
                (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
        else
            AppTitleTextBlock.Foreground =
                (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
    }
}
