using System.Collections.Generic;
using System.IO;

namespace FundMon.Repository;

public struct FundFigures
{
    public int FundID { get; set; }
    public double AverageCost { get; set; }

    public void Save(Stream s)
    {
        FileHelper.WriteInt(s, FundID);
        FileHelper.WriteDouble(s, AverageCost);
    }

    public FundFigures(int fundID, double averageCost)
    {
        FundID = fundID;
        AverageCost = averageCost;
    }

    public FundFigures(Stream s)
    {
        FundID = FileHelper.ReadInt(s);
        AverageCost = FileHelper.ReadDouble(s);
    }
}

public class Portfolio
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<FundFigures> Funds { get; set; }

    public Portfolio(int id, string name, List<FundFigures> funds, string description = "")
    {
        ID = id;
        Name = name;
        Description = description;
        Funds = funds ?? new List<FundFigures>();
    }

    public Portfolio(Stream fs)
    {
        // Achieve all reading functions that could raise exceptions before modifying members
        int id = FileHelper.ReadInt(fs);
        string name = FileHelper.ReadString(fs);
        string description = FileHelper.ReadString(fs);
        int fundsCount = FileHelper.ReadInt(fs);
        List<FundFigures> funds = new();
        for (int i = 0; i < fundsCount; i++)
            funds.Add(new FundFigures(fs));

        ID = id;
        Name = name;
        Description = description;
        Funds = funds;
    }

    public void Save(Stream fs)
    {
        FileHelper.WriteInt(fs, ID);
        FileHelper.WriteUTF8String(fs, Name);
        FileHelper.WriteUTF8String(fs, Description);
        FileHelper.WriteInt(fs, (int)Funds.Count);

        foreach (FundFigures f in Funds)
            f.Save(fs);
    }
}
