using FundMon.Pages;
using FundMon.Repository;
using FundMon.Services;
using FundMon.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace FundMon;

public sealed partial class MainWindow : Window
{
    private readonly MainWindowViewModel ViewModel;

    public MainWindow()
    {
        InitializeComponent();
        INavigationService navigationService = App.Current.Services.GetService<INavigationService>();
        navigationService.SetNavigationFrame(RootFrame);
        RestoreSizeAndPosition();
        ViewModel = new();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBar);
        Activated += MainWindow_Activated;
        Repo.Load(Config.Config.File);
        //RootFrame.Navigate(typeof(PortfoliosPage));
        navigationService.Navigate(typeof(PortfoliosPage));
        Repo.UpdateFundsHistorical();
    }

    private void RestoreSizeAndPosition()
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Graphics.SizeInt32 size = new();

        if (localSettings.Values["Width"] is double width)
            size.Width = (int)width;
        else
            return;
        if (localSettings.Values["Height"] is double height)
            size.Height= (int)height;
        else
            return;

        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // m_window in App.cs
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Resize(size);
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

    private void Window_SizeChanged(object _, WindowSizeChangedEventArgs args)
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        localSettings.Values["Width"] = args.Size.Width;
        localSettings.Values["Height"] = args.Size.Height;
    }
}
