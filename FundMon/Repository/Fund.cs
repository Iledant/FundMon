using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FundMon.Repository;

public class Fund
{
    public int ID { get; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MorningStarID { get; set; }
    public List<DateValue> Historical { get; internal set; }

    public delegate void OnHistoricalChanged();

    public OnHistoricalChanged HistoricalChanged = null;

    public Fund(int id, string name, string morningStarID = "", string description = "", List<DateValue> historical = null)
    {
        ID = id;
        Name = name;
        Description = description;
        MorningStarID = morningStarID;
        Historical = historical ?? new List<DateValue>();
    }

    public Fund(Stream fs)
    {
        // Achieve all reading functions that could raise exceptions before modifying members
        int id = FileHelper.ReadInt(fs);
        string name = FileHelper.ReadString(fs);
        string description = FileHelper.ReadString(fs);
        string morningStarID = FileHelper.ReadString(fs);
        int historicalCount = FileHelper.ReadInt(fs);
        List<DateValue> historical = new List<DateValue>();
        for (int i = 0; i < historicalCount; i++)
        {
            historical.Add(new DateValue(fs));
        }
        
        ID = id;
        Name = name;
        Description = description;
        MorningStarID = morningStarID;
        Historical = historical;
    }

    public void Save(Stream fs)
    {
        FileHelper.WriteInt(fs, ID);
        FileHelper.WriteUTF8String(fs, Name);
        FileHelper.WriteUTF8String(fs, Description);
        FileHelper.WriteUTF8String(fs, MorningStarID);
        FileHelper.WriteInt(fs, Historical.Count);
        foreach (DateValue value in Historical)
        {
            value.Save(fs);
        }
    }

    public async void FetchHistorical()
    {
        if (MorningStarID != "")
        {
            DateTime latestDate = Historical.Max(dv => dv.Date);
            List<DateValue> historical = await MorningStarHelpers.GetHistoricalFromID(MorningStarID,latestDate);
            if (historical is null)
                return;
            foreach (DateValue value in historical)
            {
                DateValue found = Historical.Find(dv => dv.Date == value.Date);
                if (found is not null)
                    Historical.Remove(found);
                Historical.Add(value);
            }
            Historical.Sort( (d1, d2) => d1.Date.CompareTo(d2.Date));
            HistoricalChanged?.Invoke();
        }
    }
}
