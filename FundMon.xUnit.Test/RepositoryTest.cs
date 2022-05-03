using Xunit;
using FundMon.Repository;
using System.IO;
using System.Collections.Generic;

namespace FundMon.xUnit.Test;

[TestCaseOrderer("FundMon.xUnit.Test.PriorityOrderer", "FundMon.xUnit.Test")]
public class RepositoryTest
{
    static readonly MemoryStream m = new();
    static readonly Repo repo = new();

    [Fact, TestPriority(1)]
    public void SaveAndLoadEmptyRepoTest()
    {
        repo.Save(m);

        m.Flush();
        m.Position = 0;

        repo.Load(m);

        Assert.Empty(repo.Funds);
        Assert.Empty(repo.Portfolios);
    }

    [Fact, TestPriority(2)]
    public void SaveAndLoadRepoWithFundsTest()
    {
        m.Position = 0;

        repo.AddFund("Fond 1", "ID Fond 1", "Description fond 1");
        repo.AddFund("Fond 2", "ID Fond 2", "Description fond 2");

        repo.Save(m);

        m.Flush();
        m.Position = 0;

        repo.Load(m);

        Assert.Equal(2, repo.Funds.Count);
        Assert.Empty(repo.Portfolios);
    }

    [Fact, TestPriority(3)]
    public void FundUpdateTest()
    {
        Assert.Equal(1, repo.Funds[0].ID);
        repo.UpdateFund(1,"Fond modifié","Description fond modifié");
        Assert.Equal("Fond modifié", repo.Funds[0].Name);
        Assert.Equal("Description fond modifié", repo.Funds[0].Description);
    }

    [Fact, TestPriority(4)]
    public void SaveAndLoadRepoWithFundsAndPortfoliosTest()
    {
        m.Position = 0;

        repo.AddPortfolio("Portefeuille 1", "Description portefeuille 1");
        repo.AddPortfolio("Portefeuille 2", "Description portefeuille 2");

        repo.Save(m);

        m.Flush();
        m.Position=0;

        repo.Load(m);

        Assert.Equal(2,repo.Funds.Count);
        Assert.Equal(2,repo.Portfolios.Count);
    }

    [Fact, TestPriority(5)]
    public void UpdatePortfolioTest()
    {
        Assert.Equal(1, repo.Portfolios[0].ID);
        repo.UpdatePortfolio(1, "Portefeuille modifié", "Description modifiée");
        Assert.Equal("Portefeuille modifié", repo.Portfolios[0].Name);
        Assert.Equal("Description modifiée", repo.Portfolios[0].Description);
    }

    [Fact, TestPriority(6)]
    public void EmptyPortfolioFundsTest()
    {
        Assert.Equal(1, repo.Portfolios[0].ID);
        List<Fund> portfolioFunds = repo.PortfolioFunds(1);
        Assert.Empty(portfolioFunds);
    }

    [Fact,TestPriority(7)]
    public void OneFundPortfolioTest()
    {
        Assert.Equal(2,repo.Funds[1].ID);
        repo.AddFundToPortfolio(1, 2,2.5);
        Assert.Single(repo.Portfolios[0].Funds);
        List<Fund> portfolioFunds = repo.PortfolioFunds(1);
        Assert.Single(portfolioFunds);
        Assert.Equal(2,repo.Portfolios[0].Funds[0].FundID);
    }

    [Fact, TestPriority(8)]
    public void RemovePortfolioTest()
    {
        repo.RemovePortfolio(3);
        Assert.Equal(2,repo.Portfolios.Count);
        repo.RemovePortfolio(2);
        Assert.Single(repo.Portfolios);
    }

    [Fact, TestPriority(9)]
    public void RemoveFundTest()
    {
        repo.RemoveFund(3);
        Assert.Equal(2, repo.Funds.Count);
        repo.RemoveFund(2);
        Assert.Single(repo.Funds);
        Assert.Empty(repo.Portfolios[0].Funds);
    }
}
