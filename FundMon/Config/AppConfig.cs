using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FundMon.Config;

public static class AppConfig
{
    static readonly private FileStream file;

    static public FileStream File { get => file; }

    static AppConfig()
    {
        var configFilePath = ApplicationData.Current.LocalFolder;
        file = new FileStream(Path.Combine(configFilePath.Path, "fundmon.fmf"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    static public void SaveAndClose()
    {
        file.Flush();
        file.Close();
    }
}
