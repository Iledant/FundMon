using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace FundMon.Config;

public class LogEventArgs
{
    public LogEventArgs(string text) { Text = text; }
    public string Text { get; }
}

public static class Config

{
    static readonly private FileStream file;

    static public FileStream File { get => file; }

    static readonly private ObservableCollection<(DateTime, string)> log = new();

    static public ObservableCollection<(DateTime, string)> Log { get => log; }

    public delegate void LogEventHandler(object sender, LogEventArgs e);

    static public event LogEventHandler LogAdded;

    static Config()
    {
        var configFilePath = ApplicationData.Current.LocalFolder;
        file = new FileStream(Path.Combine(configFilePath.Path, "fundmon.fmf"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    static public void SaveAndClose()
    {
        file.Flush();
        file.Close();
    }

    static public void AddLog(string message)
    {
        Log.Add((DateTime.Now, message));
        LogAdded?.Invoke(null, new LogEventArgs(message));
    }
}
