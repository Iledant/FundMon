﻿using Xunit;
using FundMon.Repository;
using System.IO;
using System.Collections.Generic;
using System;

namespace FundMon.xUnit.Test;

[TestCaseOrderer("FundMon.xUnit.Test.PriorityOrderer", "FundMon.xUnit.Test")]
public class RepositoryTest
{
    static readonly MemoryStream m = new();
    static readonly ModeFixture mode = new();

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
    public async void SaveAndLoadRepoWithFundsTest()
    {
        m.Position = 0;

        await Repo.AddFund("Fond 1", "ID Fond 1", "Description fond 1");
        await Repo.AddFund("Fond 2", "ID Fond 2", "Description fond 2");

        Repo.Save(m);

        m.Flush();
        m.Position = 0;

        Repo.Load(m);

        Assert.Empty(Repo.Funds);
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

        Assert.Empty(Repo.Funds);
        Assert.Equal(2,Repo.Portfolios.Count);
    }

    [Fact, TestPriority(4)]
    public void UpdatePortfolioTest()
    {
        Assert.Equal(1, Repo.Portfolios[0].ID);
        Repo.UpdatePortfolio(1, "Portefeuille modifié", "Description modifiée");
        Assert.Equal("Portefeuille modifié", Repo.Portfolios[0].Name);
        Assert.Equal("Description modifiée", Repo.Portfolios[0].Description);
    }

    [Fact, TestPriority(5)]
    public void EmptyPortfolioFundsTest()
    {
        Assert.Equal(1, Repo.Portfolios[0].ID);
        Assert.Empty(Repo.Portfolios[0].Funds);
    }

    [Fact,TestPriority(6)]
    public async void OneFundPortfolioTest()
    {
        await Repo.AddFund("Fond 1", "MonrnigStar", "Description fond 1");

        Repo.AddFundToPortfolio(Repo.Portfolios[0], Repo.Funds[0], 2.5);
        Assert.Single(Repo.Portfolios[0].Funds);
        Assert.Equal(Repo.Funds[0].ID, Repo.Portfolios[0].Funds[0].Fund.ID);
    }

    [Fact, TestPriority(7)]
    public void FundUpdateTest()
    {
        Assert.Equal(1, Repo.Funds[0].ID);
        Repo.UpdateFund(1, "Fond modifié", "Description fond modifié");
        Assert.Equal("Fond modifié", Repo.Funds[0].Name);
        Assert.Equal("Description fond modifié", Repo.Funds[0].Description);
    }

    [Fact, TestPriority(8)]
    public void RemovePortfolioTest()
    {
        Repo.RemovePortfolio(Repo.Portfolios[1]);
        Assert.Single(Repo.Portfolios);
    }

    [Fact, TestPriority(9)]
    public async void RemoveFundTest()
    {
        await Repo.AddFund("Fond 2", "MonrnigStar", "Description fond 2");

        Repo.RemoveFund(3);
        Assert.Equal(2, Repo.Funds.Count);
        Repo.RemoveFund(2);
        Assert.Single(Repo.Funds);
    }

    [Fact, TestPriority(10)]
    public void SaveAndLoadRepoWithFundsHistoricalAndPortfoliosTest()
    {

        Assert.Single(Repo.Funds);
        Assert.Single(Repo.Portfolios);
        Repo.Funds[0].Historical.Add(new DateValue(12.5, new DateTime(2020, 4, 1)));

        MemoryStream testStream = new();

        Repo.Save(testStream);

        testStream.Position = 0;

        Repo.Load(testStream);

        Assert.Single(Repo.Funds);
        Assert.Single(Repo.Portfolios);
        Assert.Single(Repo.Funds[0].Historical);
    }

}
