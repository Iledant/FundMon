using FundMon.Repository;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace FundMon.xUnit.Test;

public class PortfolioTest
{
    [Fact]
    public void SaveAndLoadTest()
    {
        Portfolio p = new(1, "Portfolio 1", new List<FundPerformance>(), "Description 1");

        MemoryStream m = new();
        p.Save(m);

        m.Flush();
        m.Position = 0;

        Portfolio p2 = new(m, new List<Fund>());

        m.Close();
        Assert.Equal(p.ID, p2.ID);
        Assert.Equal(p.Name, p2.Name);
        Assert.Equal(p.Description, p2.Description);
        Assert.Equal(p.Funds.Count, p2.Funds.Count);
    }
}
