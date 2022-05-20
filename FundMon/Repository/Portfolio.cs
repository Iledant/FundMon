using FundMon.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FundMon.Repository;

public class Portfolio : Bindable
{
    private int id;
    private string name;
    private string description;

    public int ID
    {
        get => id;
        init
        {
            id = value;
            OnPropertyChanged(nameof(ID));
        }
    }
    public string Name { 
        get => name; 
        set {
            name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    public string Description
    {
        get => description;
        set
        {
            description = value;
            OnPropertyChanged(nameof(Description));
        }
    }
    public ObservableCollection<FundPerformance> Funds { get; internal set; }

    public Portfolio(int id, string name, List<FundPerformance> funds = null, string description = "")
    {
        ID = id;
        Name = name;
        Description = description;
        Funds = funds is null ? new () : new ObservableCollection<FundPerformance>(funds);
    }

    public Portfolio(Stream fs, List<Fund> fundCollection)
    {
        // Achieve all reading functions that could raise exceptions before modifying members
        int id = FileHelper.ReadInt(fs);
        string name = FileHelper.ReadString(fs);
        string description = FileHelper.ReadString(fs);
        int fundsCount = FileHelper.ReadInt(fs);
        List<FundPerformance> funds = new();
        for (int i = 0; i < fundsCount; i++)
            funds.Add(new FundPerformance(fs, fundCollection));

        ID = id;
        Name = name;
        Description = description;
        Funds = new ObservableCollection<FundPerformance>(funds);
    }

    public void Save(Stream fs)
    {
        FileHelper.WriteInt(fs, ID);
        FileHelper.WriteUTF8String(fs, Name);
        FileHelper.WriteUTF8String(fs, Description);
        FileHelper.WriteInt(fs, (int)Funds.Count);

        foreach (FundPerformance f in Funds)
            f.Save(fs);
    }
}
