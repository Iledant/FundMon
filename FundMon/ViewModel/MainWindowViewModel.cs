using CommunityToolkit.Mvvm.ComponentModel;
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
    public ObservableCollection<(DateTime, string,string)> Log => Config.Config.Log;

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(IsOpen))]
    private string lastLog = "";

    [ObservableProperty]
    private string lastKind = "";

    public bool IsOpen => (DateTime.Now - lastChanged).TotalSeconds < 2;

    private DateTime lastChanged = DateTime.MinValue;
    private PeriodicTimer timer;

    private void LogAdded(object sender, Config.LogEventArgs e)
    {
        LastLog = e.Text;
        LastKind = e.Kind ?? "";
        lastChanged = DateTime.Now;
    }

    public MainWindowViewModel()
    {
        Config.Config.LogAdded += LogAdded;
        timer = new(TimeSpan.FromSeconds(1));
        PeriodicUpdate();
    }

    private async void PeriodicUpdate()
    {
        while (await timer.WaitForNextTickAsync())
            OnPropertyChanged(nameof(IsOpen));
    }
}
