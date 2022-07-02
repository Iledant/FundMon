using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    public static ObservableCollection<(DateTime, string,string)> Log => AppConfig.Log;

    [ObservableProperty]
    private string lastLog = "";

    [ObservableProperty]
    private string lastKind = "";

    [ObservableProperty]
    private bool isOpen =false;

    private DateTime lastChanged;

    private void LogAdded(object sender, Config.LogEventArgs e)
    {
        LastLog = e.Text;
        LastKind = e.Kind ?? "";
        lastChanged = DateTime.Now;
        ShowInfoBar();
    }

    public MainWindowViewModel()
    {
        AppConfig.LogAdded += LogAdded;
    }

    private async void ShowInfoBar()
    {
        IsOpen = true;
        async Task<bool> Debounce()
        {
            DateTime changed = lastChanged;
            await Task.Delay(2000);
            return changed != lastChanged;
        }

        if (await Debounce())
            return;

        IsOpen = false;
    }
}
