using CommunityToolkit.Mvvm.Input;
using FundMon.Config;
using FundMon.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;

namespace FundMon.ViewModel;

public partial class LogViewModel
{
    public ObservableCollection<LogEntry> Logs => AppConfig.Log;
    public IRelayCommand GoBackCommand { get; }

    private readonly INavigationService navigationService;

    private void GoBack() => navigationService.GoBack();

    public LogViewModel()
    {
        navigationService = App.Current.Services.GetService<INavigationService>();
        GoBackCommand = new RelayCommand(GoBack);
    }
}
