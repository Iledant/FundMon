using Xunit;
using FundMon.Repository;
using System.IO;

namespace FundMon.xUnit.Test;

public class FundTest
{
    [Fact]
    public void SaveAndLoadTest()
    {
        Fund f = new(0,"Fond 1","MorningStar 1", "Description 1");
        MemoryStream m = new();
        f.Save(m);

        m.Flush();
        m.Position = 0;

        Fund f2 = new(m);

        m.Close();

        Assert.Equal(f.ID, f2.ID);
        Assert.Equal(f.Name, f2.Name);
        Assert.Equal(f.Description, f2.Description);
        Assert.Equal(f.MorningStarID, f2.MorningStarID);
    }
}
