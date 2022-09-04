using System;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;

namespace FundMon.Config;

public class LogEventArgs
{
    public LogEventArgs(string text, string kind = "")
    {
        Text = text;
        Kind = kind;
    }
    public string Text { get; }
    public string Kind { get; }
}

public static class AppMode
{
    static public bool IsTestRunning = false;
}

public record LogEntry(DateTime Time, string Message,string Kind);

public static class AppConfig

{
    static readonly private FileStream file;

    static public FileStream File { get => file; }

    static readonly private ObservableCollection<LogEntry> log = new();

    static public ObservableCollection<LogEntry> Log { get => log; }

    public delegate void LogEventHandler(object sender, LogEventArgs e);

    static public event LogEventHandler LogAdded;

    static AppConfig()
    {
        if (!AppMode.IsTestRunning)
        {
            StorageFolder configFilePath = ApplicationData.Current.LocalFolder;
            file = new FileStream(Path.Combine(configFilePath.Path, "fundmon.fmf"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
    }

    static public void SaveAndClose()
    {
        if (file is not null)
        {
            file.Flush();
            file.Close();
        }
    }

    static public void AddLog(string message,string kind)
    {
        Log.Add(new(DateTime.Now, message, kind));
        LogAdded?.Invoke(null, new LogEventArgs(message,kind));
    }
}
