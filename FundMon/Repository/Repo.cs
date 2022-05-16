using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FundMon.Repository;

public static class Repo
{
    public static List<Portfolio> Portfolios { get; internal set; } = new();
    public static List<Fund> Funds { get; internal set; } = new();
    private const string Header = "FundMon 1.0";
    private static int maxFundID = 1;
    private static int maxPortfolioID = 1;

    public static void Save(Stream fs)
    {
        fs.Flush();
        fs.SetLength(0);
        fs.Position = 0;
        byte[] headerBytes = UTF8Encoding.UTF8.GetBytes(Header);
        fs.Write(headerBytes, 0, headerBytes.Length);

        FileHelper.WriteInt(fs, Portfolios.Count);
        foreach (Portfolio p in Portfolios)
            p.Save(fs);

        FileHelper.WriteInt(fs, Funds.Count);
        foreach (Fund f in Funds)
            f.Save(fs);

        fs.Flush();
    }

    public static List<Fund> PortfolioFunds(int portfolioID)
    {
        List<Fund> funds = new();
        Portfolio portfolio = Portfolios.Find(p => p.ID == portfolioID);

        if (portfolio == null)
            return funds;

        foreach (FundFigures fundFigure in portfolio.Funds)
        {
            Fund f = Funds.Find(f => f.ID == fundFigure.FundID);
            if (f is not null)
                funds.Add(f);
        }
        return funds;
    }

    public static List<FundPerformance> PortfolioPerformance(int portfolioID)
    {
        List<FundPerformance> fundPerformances = new();
        Portfolio portfolio = Portfolios.Find(p => p.ID == portfolioID);
        if (portfolio is null)
            return fundPerformances;

        foreach (FundFigures ff in portfolio.Funds)
        {
            Fund fund = Funds.Find(f => f.ID == ff.FundID);
            if (fund is not null)
                fundPerformances.Add(new FundPerformance(fund, ff.AverageCost));
        }
        return fundPerformances;
    }

    public static int AddFund(string name, string morningstarID, string description = "")
    {
        Fund f = new(maxFundID, name, morningstarID, description);
        Funds.Add(f);
        maxFundID++;
        return maxFundID - 1;
    }

    public static void UpdateFund(int fundID, string name, string description)
    {
        Fund f = Funds.Find(f => f.ID == fundID);
        if (f == null)
            return;

        f.Name = name;
        f.Description = description;
    }

    public static void RemoveFund(int fundID)
    {
        Fund fund = Funds.Find(f => f.ID == fundID);

        if (fund is null)
            return;

        Funds.Remove(fund);

        foreach (Portfolio p in Portfolios)
            p.Funds.RemoveAll(f => f.FundID == fundID);
    }

    public static void AddPortfolio(string name, string description = "")
    {
        Portfolio p = new(maxPortfolioID, name, new List<FundFigures>(), description);
        Portfolios.Add(p);
        maxPortfolioID++;
    }

    public static void UpdatePortfolio(int portfolioID, string name, string description)
    {
        Portfolio p = Portfolios.Find(f => f.ID == portfolioID);
        if (p is null)
            return;

        p.Name = name;
        p.Description = description;
    }

    public static void RemovePortfolio(int portfolioID)
    {
        Portfolio portfolio = Portfolios.Find(p => p.ID == portfolioID);

        if (portfolio is not null)
            Portfolios.Remove(portfolio);
    }

    public static void AddFundToPortfolio(int portfolioID, int fundID, double averageCost)
    {
        Portfolio p = Portfolios.Find(p => p.ID == portfolioID);

        if (p is null)
            return;

        p.Funds.Add(new FundFigures(fundID, averageCost));
    }

    public static void UpdateFundAverageCost(int portfolioID, int fundID, double newAverageCost)
    {
        Portfolio p = Portfolios.Find(p => p.ID == portfolioID);

        if (p is null)
            return;

        FundFigures f = p.Funds.Find(f => f.FundID == fundID);

        if (f is null)
            return;

        f.AverageCost = newAverageCost;
    }

    public static void Load(Stream fs)
    {
        ReadAndCheckHeader(fs);
        List<Portfolio> portfolios = ReadPortfolios(fs);
        List<Fund> funds = ReadFunds(fs);
        CalculateMaxPortfolioID(portfolios);
        CalculateMaxFundID(funds);

        Portfolios = portfolios;
        Funds = funds;
    }

    private static List<Fund> ReadFunds(Stream fs)
    {
        List<Fund> funds = new();
        int fundsCount = FileHelper.ReadInt(fs);
        for (int i = 0; i < fundsCount; i++)
            funds.Add(new(fs));

        return funds;
    }

    private static  List<Portfolio> ReadPortfolios(Stream fs)
    {
        List<Portfolio> portfolios = new();
        int portfoliosCount = FileHelper.ReadInt(fs);
        for (int i = 0; i < portfoliosCount; i++)
            portfolios.Add(new(fs));

        return portfolios;
    }

    private static void ReadAndCheckHeader(Stream fs)
    {
        byte[] headerBytes = UTF8Encoding.UTF8.GetBytes(Header);
        fs.Read(headerBytes, 0, headerBytes.Length);
        byte[] checkHeaderBytes = UTF8Encoding.UTF8.GetBytes(Header);

        if (!checkHeaderBytes.SequenceEqual(headerBytes))
            throw new Exception("Erreur de header");
    }

    private static void CalculateMaxFundID(List<Fund> funds)
    {
        if (funds.Count == 0)
        {
            maxFundID = 1;
            return;
        }
        maxFundID = funds.Max(f => f.ID) + 1;
    }

    private static void CalculateMaxPortfolioID(List<Portfolio> portfolios)
    {
        if (portfolios.Count == 0)
        {
            maxPortfolioID = 1;
            return;
        }
        maxPortfolioID = portfolios.Max(p => p.ID) + 1;
    }

    public static async void UpdateFundsHistorical()
    {
        foreach (Fund f in Funds)
        {
            f.Historical = await MorningStarHelpers.GetHistoricalFromID(f.MorningStarID);
        }
    }
}