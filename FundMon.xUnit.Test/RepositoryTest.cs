using Xunit;
using FundMon.Repository;
using System.IO;
using System.Collections.Generic;

namespace FundMon.xUnit.Test;

[TestCaseOrderer("FundMon.xUnit.Test.PriorityOrderer", "FundMon.xUnit.Test")]
public class RepositoryTest
{
    static readonly MemoryStream m = new();

    [Fact, TestPriority(1)]
    public void SaveAndLoadEmptyRepoTest()
    {
        Repo.Save(m);

        m.Flush();
        m.Position = 0;

        Repo.Load(m);

        Assert.Empty(Repo.Funds);
        Assert.Empty(Repo.Portfolios);
    }

    [Fact, TestPriority(2)]
    public void SaveAndLoadRepoWithFundsTest()
    {
        m.Position = 0;

        Repo.AddFund("Fond 1", "ID Fond 1", "Description fond 1");
        Repo.AddFund("Fond 2", "ID Fond 2", "Description fond 2");

        Repo.Save(m);

        m.Flush();
        m.Position = 0;

        Repo.Load(m);

        Assert.Equal(2, Repo.Funds.Count);
        Assert.Empty(Repo.Portfolios);
    }

    [Fact, TestPriority(3)]
    public void SaveAndLoadRepoWithFundsAndPortfoliosTest()
    {
        m.Position = 0;

        Repo.AddPortfolio("Portefeuille 1", "Description portefeuille 1");
        Repo.AddPortfolio("Portefeuille 2", "Description portefeuille 2");

        Repo.Save(m);

        m.Flush();
        m.Position=0;

        Repo.Load(m);

        Assert.Equal(2,Repo.Funds.Count);
        Assert.Equal(2,Repo.Portfolios.Count);
    }

    [Fact, TestPriority(4)]
    public void EmptyPortfolioFundsTest()
    {
        Assert.Equal(1, Repo.Portfolios[0].ID);
        List<Fund> portfolioFunds = Repo.PortfolioFunds(1);
        Assert.Empty(portfolioFunds);
    }

    [Fact,TestPriority(5)]
    public void OneFundPortfolioTest()
    {
        Assert.Equal(2,Repo.Funds[1].ID);
        Repo.AddFundToPortfolio(1, 2,2.5);
        Assert.Single(Repo.Portfolios[0].Funds);
        List<Fund> portfolioFunds = Repo.PortfolioFunds(1);
        Assert.Single(portfolioFunds);
        Assert.Equal(2,Repo.Portfolios[0].Funds[0].FundID);
    }

    [Fact, TestPriority(6)]
    public void RemovePortfolioTest()
    {
        Repo.RemovePortfolio(3);
        Assert.Equal(2,Repo.Portfolios.Count);
        Repo.RemovePortfolio(2);
        Assert.Single(Repo.Portfolios);
    }

    [Fact, TestPriority(7)]
    public void RemoveFundTest()
    {
        Repo.RemoveFund(3);
        Assert.Equal(2, Repo.Funds.Count);
        Repo.RemoveFund(2);
        Assert.Single(Repo.Funds);
        Assert.Empty(Repo.Portfolios[0].Funds);
    }
}
