using FundMon.Repository;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace FundMon.xUnit.Test;

public class FundTest
{
    private readonly Fund fundTest = new(0, "Fond 1", "MorningStar 1", "Description 1");
    private readonly Fund fundTestWithHistorical = new(1, "Fond 2", "MorningStar 2", "Description 2", new()
    {
        new DateValue(1.0, new System.DateTime(2022, 1, 1)),
        new DateValue(2.0, new System.DateTime(2022, 2, 3))
    });

    [Fact]
    public void SaveAndLoadTest()
    {
        MemoryStream m = new();
        fundTest.Save(m);

        m.Flush();
        m.Position = 0;

        Fund f2 = new(m);

        m.Close();

        Assert.Equal(fundTest.ID, f2.ID);
        Assert.Equal(fundTest.Name, f2.Name);
        Assert.Equal(fundTest.Description, f2.Description);
        Assert.Equal(fundTest.MorningStarID, f2.MorningStarID);
    }

    [Fact]
    public void SaveAndLoadTestWithHistorical()
    {
        MemoryStream ms = new();
        fundTestWithHistorical.Save(ms);

        ms.Flush();
        ms.Position = 0;

        Fund f2 = new(ms);

        ms.Close();

        Assert.Equal(fundTestWithHistorical.ID, f2.ID);
        Assert.Equal(fundTestWithHistorical.Name, f2.Name);
        Assert.Equal(fundTestWithHistorical.Description, f2.Description);
        Assert.Equal(fundTestWithHistorical.MorningStarID, f2.MorningStarID);
        Assert.Equal(fundTestWithHistorical.Historical.Count, f2.Historical.Count);
        Assert.Equal(fundTestWithHistorical.Historical[0].Date, f2.Historical[0].Date);
        Assert.Equal(fundTestWithHistorical.Historical[0].Value, f2.Historical[0].Value);
        Assert.Equal(fundTestWithHistorical.Historical[1].Date, f2.Historical[1].Date);
        Assert.Equal(fundTestWithHistorical.Historical[1].Value, f2.Historical[1].Value);
    }
}
