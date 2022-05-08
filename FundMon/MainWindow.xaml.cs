﻿using FundMon.Config;
using FundMon.Pages;
using FundMon.Repository;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FundMon;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        Repo.Load(AppConfig.File);
        RootFrame.Navigate(typeof(PortfoliosPage));
        Repo.UpdateFundsHistorical();
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        Repo.Save(AppConfig.File);
        AppConfig.SaveAndClose();
    }
}
