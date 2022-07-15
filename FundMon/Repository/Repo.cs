using FundMon.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.Repository;

public static class Repo
{
    public static ObservableCollection<Portfolio> Portfolios { get; internal set; } = new();
    public static ObservableCollection<Fund> Funds { get; internal set; } = new();
    private const string Header = "FundMon 1.0";
    private static int maxFundID = 1;
    private static int maxPortfolioID = 1;

    public static void Save(Stream fs)
    {
        fs.Flush();
        fs.SetLength(0);
        fs.Position = 0;
        byte[] headerBytes = Encoding.UTF8.GetBytes(Header);
        fs.Write(headerBytes, 0, headerBytes.Length);

        int count = Funds.Count<Fund>(f => f.LinkCount > 0);
        FileHelper.WriteInt(fs, count);
        foreach (Fund f in Funds)
            if (f.LinkCount > 0)
                f.Save(fs);

        FileHelper.WriteInt(fs, Portfolios.Count);
        foreach (Portfolio p in Portfolios)
            p.Save(fs);

        fs.Flush();
    }

    public static async Task<Fund> AddFund(string name, string morningstarID, string description = "")
    {
        List<DateValue> values = null;
        if (morningstarID is not null && morningstarID != "")
        {
            values = await MorningStarHelpers.GetCompactHistoricalFromID(morningstarID);
        }
        Fund f = new(maxFundID, name, morningstarID, description, values);
        Funds.Add(f);
        maxFundID++;
        return f;
    }

    public static void UpdateFund(int fundID, string name, string description)
    {
        for (int i = 0; i < Funds.Count; i++)
        {
            if (Funds[i].ID == fundID)
            {
                Funds[i].Name = name;
                Funds[i].Description = description;
                break;
            }
        }
    }

    public static void RemoveFund(int fundID)
    {
        Fund fund = null;
        for (int i = 0; i < Funds.Count; i++)
        {
            if (Funds[i].ID == fundID)
            {
                fund = Funds[i];
                break;
            }
        }

        if (fund is null)
            return;

        Funds.Remove(fund);

        foreach (Portfolio p in Portfolios)
        {
            for (int i = 0; i < p.Funds.Count; i++)
            {
                if (p.Funds[i].Fund == fund)
                {
                    p.Funds.RemoveAt(i);
                }
            }
        }
    }

    public static void AddPortfolio(string name, string description = "")
    {
        Portfolio p = new(maxPortfolioID, name, new List<FundPerformance>(), description);
        Portfolios.Add(p);
        maxPortfolioID++;
    }

    public static void UpdatePortfolio(int portfolioID, string name, string description)
    {
        for (int i = 0; i < Portfolios.Count; i++)
        {
            if (Portfolios[i].ID == portfolioID)
            {
                Portfolios[i].Name = name;
                Portfolios[i].Description = description;
                break;
            }
        }
    }

    public static void RemovePortfolio(Portfolio portfolio)
    {
        Portfolios.Remove(portfolio);
    }

    public static void AddFundToPortfolio(Portfolio portfolio, Fund fund, double averageCost)
    {
        portfolio.Funds.Add(new(fund, averageCost));
        fund.LinkCount++;
    }

    public static void UpdateFundAverageCost(Portfolio portfolio, int fundID, double newAverageCost)
    {
        if (portfolio is null)
            return;
        
        for (int i = 0; i < portfolio.Funds.Count; i++)
        {
            if (portfolio.Funds[i].Fund.ID == fundID)
            {
                portfolio.Funds[i].AverageCost = newAverageCost;
                break;
            }
        }
    }

    public static void RemoveFundFromPortfolio(Portfolio portfolio, FundPerformance fundPerformance)
    {
        if (!portfolio.RemoveFund(fundPerformance))
            return;
        fundPerformance.Fund.LinkCount--;
        if (fundPerformance.Fund.LinkCount == 0)
            Funds.Remove(fundPerformance.Fund); 
    }

    public static void Load(Stream fs)
    {
        ReadAndCheckHeader(fs);
        List<Fund> funds = ReadFunds(fs);
        List<Portfolio> portfolios = ReadPortfolios(fs, funds);
        CalculateMaxPortfolioID(portfolios);
        CalculateMaxFundID(funds);

        funds = funds.FindAll(f => f.LinkCount > 0);

        Portfolios = new ObservableCollection<Portfolio>(portfolios);
        Funds = new ObservableCollection<Fund>(funds);
    }

    private static List<Fund> ReadFunds(Stream fs)
    {
        List<Fund> funds = new();
        int fundsCount = FileHelper.ReadInt(fs);
        for (int i = 0; i < fundsCount; i++)
            funds.Add(new(fs));

        return funds;
    }

    private static List<Portfolio> ReadPortfolios(Stream fs, List<Fund> funds)
    {
        List<Portfolio> portfolios = new();
        int portfoliosCount = FileHelper.ReadInt(fs);
        for (int i = 0; i < portfoliosCount; i++)
            portfolios.Add(new(fs, funds));

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
            int count = await f.FetchHistorical();
            AppConfig.AddLog($"Historique de {f.Name} mis à jour, {count} valeurs ajoutées","Info");
        }
    }
}