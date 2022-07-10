using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FundMon.Repository;

public partial class Portfolio : ObservableObject
{
    private int id;

    [ObservableProperty]
    private string name;
    
    [ObservableProperty]
    private string description;

    public int ID
    {
        get => id;
        init => SetProperty(ref id, value);
    }

    private ObservableCollection<FundPerformance> funds;

    public ObservableCollection<FundPerformance> Funds { 
        get => funds; 
        internal set => SetProperty(ref funds, value); 
    }

    public Portfolio(int id, string name, List<FundPerformance> fundPerformances = null, string description = "")
    {
        ID = id;
        Name = name;
        Description = description;
        Funds = fundPerformances is null ? new () : new ObservableCollection<FundPerformance>(fundPerformances);
    }

    public Portfolio(Stream fs, List<Fund> fundCollection)
    {
        // Achieve all reading functions that could raise exceptions before modifying members
        int id = FileHelper.ReadInt(fs);
        string name = FileHelper.ReadString(fs);
        string description = FileHelper.ReadString(fs);
        int fundsCount = FileHelper.ReadInt(fs);
        List<FundPerformance> readFunds = new();
        for (int i = 0; i < fundsCount; i++)
            readFunds.Add(new FundPerformance(fs, fundCollection));

        ID = id;
        Name = name;
        Description = description;
        Funds = new ObservableCollection<FundPerformance>(readFunds);
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

    public bool RemoveFund(FundPerformance fund)
    {
        if (fund is null)
            return false;

        for (int i = 0; i< Funds.Count; i++)
        {
            if (Funds[i].Fund.ID == fund.Fund.ID && Funds[i].AverageCost == fund.AverageCost)
            {
                Funds[i].Fund.LinkCount--;
                Funds.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
