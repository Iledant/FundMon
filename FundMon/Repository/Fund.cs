using System.Collections.Generic;
using System.IO;

namespace FundMon.Repository;

public class Fund
{
    public int ID { get; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MorningStarID { get; set; }
    public List<DateValue> Historical { get; internal set; }

    public Fund(int id, string name, string morningStarID = "", string description = "")
    {
        ID = id;
        Name = name;
        Description = description;
        MorningStarID = morningStarID;
        Historical = new List<DateValue>();
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
            Historical = await MorningStarHelpers.GetHistoricalFromID(MorningStarID);
    }
}
