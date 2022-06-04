using FundMon.Repository;
using FundMon.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace FundMon.Pages;

public sealed partial class FundChart : Page
{
    private FundChartViewmodel ViewModel;
    public FundChart()
    {
        InitializeComponent();
        ViewModel = new();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is FundPerformance fund)
        {
            ViewModel.Fund = fund;
            TitleTextBox.Text = "Fond " + ViewModel.Fund.Fund.Name;
        }
        else
        {
            throw new Exception("FundPerformance navigation parameter expected");
        }
        
        base.OnNavigatedTo(e);
    }
}
