using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FundMon.Repository;

public partial class Fund : ObservableObject
{
    private int id;

    [ObservableProperty]
    private string name;
    
    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private string morningStarID;

    [ObservableProperty]
    private int linkCount = 0;
    
    public int ID
    {
        get => id;
        init => SetProperty(ref id, value);
    }

    [ObservableProperty]
    private ObservableCollection<DateValue> historical;

    public Fund(int id, string name, string morningStarID = "", string description = "", List<DateValue> historical = null, int linkCount = 0)
    {
        ID = id;
        Name = name;
        Description = description;
        MorningStarID = morningStarID;
        Historical = historical is null ? new() : new(historical);
        LinkCount = linkCount;
    }

    public Fund(Stream fs)
    {
        // Achieve all reading functions that could raise exceptions before modifying members
        int id = FileHelper.ReadInt(fs);
        string name = FileHelper.ReadString(fs);
        string description = FileHelper.ReadString(fs);
        string morningStarID = FileHelper.ReadString(fs);
        int historicalCount = FileHelper.ReadInt(fs);
        List<DateValue> historical = new();
        for (int i = 0; i < historicalCount; i++)
        {
            historical.Add(new DateValue(fs));
        }

        ID = id;
        Name = name;
        Description = description;
        MorningStarID = morningStarID;
        Historical = new ObservableCollection<DateValue>(historical);
    }

    public void Save(Stream fs)
    {
        FileHelper.WriteInt(fs, ID);
        FileHelper.WriteUTF8String(fs, Name);
        FileHelper.WriteUTF8String(fs, Description);
        FileHelper.WriteUTF8String(fs, MorningStarID);
        FileHelper.WriteInt(fs, Historical.Count);
        foreach (DateValue value in Historical)
            value.Save(fs);
    }

    public async void FetchHistorical()
    {
        if (MorningStarID == "")
            return;

        DateTime latestDate = Historical.Max(dv => dv.Date);
        List<DateValue> historical = await MorningStarHelpers.GetHistoricalFromID(MorningStarID, latestDate);
        if (historical is null)
            return;
        
        List<DateValue> oldHistorical = Historical.ToList();
        foreach (DateValue value in historical)
        {
            for (int i = 0; i < oldHistorical.Count; i++)
            {
                if (oldHistorical[i].Date == value.Date)
                {
                    oldHistorical.RemoveAt(i);
                    break;
                }
            }
            oldHistorical.Add(value);
        }
        oldHistorical.Sort((a, b) => DateTime.Compare(a.Date, b.Date));
        Historical = new ObservableCollection<DateValue>(oldHistorical);
    }
}
