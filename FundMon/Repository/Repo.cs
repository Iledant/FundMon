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

        FileHelper.WriteInt(fs, Funds.Count);
        foreach (Fund f in Funds)
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
            values = await MorningStarHelpers.GetHistoricalFromID(morningstarID);
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

    public static void RemovePortfolio(int portfolioID)
    {
        for (int i = 0; i < Portfolios.Count; i++)
        {
            if (Portfolios[i].ID == portfolioID)
            {
                Portfolios.RemoveAt(i);
                break;
            }
        }
    }

    public static void AddFundToPortfolio(int portfolioID, Fund fund, double averageCost)
    {
        for (int i = 0; i < Portfolios.Count; i++)
        {
            if (Portfolios[i].ID == portfolioID)
            {
                Portfolios[i].Funds.Add(new(fund, averageCost));
                break;
            }
        }
    }

    public static void UpdateFundAverageCost(int portfolioID, int fundID, double newAverageCost)
    {
        Portfolio p = null;
        for (int i = 0; i < Portfolios.Count; i++)
        {
            if (Portfolios[i].ID == portfolioID)
            {
                p = Portfolios[i];
                break;
            }
        }

        if (p is null)
            return;

        for (int i = 0; i < p.Funds.Count; i++)
        {
            if (p.Funds[i].Fund.ID == fundID)
            {
                p.Funds[i].AverageCost = newAverageCost;
                break;
            }
        }
    }

    public static void Load(Stream fs)
    {
        ReadAndCheckHeader(fs);
        List<Fund> funds = ReadFunds(fs);
        List<Portfolio> portfolios = ReadPortfolios(fs, funds);
        CalculateMaxPortfolioID(portfolios);
        CalculateMaxFundID(funds);

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
            var historical = await MorningStarHelpers.GetHistoricalFromID(f.MorningStarID);
            f.Historical = new ObservableCollection<DateValue>(historical);
            Config.Config.AddLog($"Historique de {f.Name} mis à jour");
        }
    }
}