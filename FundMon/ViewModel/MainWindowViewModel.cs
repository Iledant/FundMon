using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<(DateTime, string)> Log => Config.Config.Log;

    [ObservableProperty]
    private string lastLog = "";

    private void LogAdded(object sender, Config.LogEventArgs e)
    {
        LastLog = e.Text;
    }

    public MainWindowViewModel()
    {
        Config.Config.LogAdded += LogAdded;
    }
}
