using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundMon.Repository;

public class Repo
{
    public List<Portfolio> Portfolios { get; internal set; }
    public List<Fund> Funds { get; internal set; }
    private const string Header = "FundMon 1.0";
    private int maxFundID = 1;
    private int maxPortfolioID = 1;

    public Repo()
    {
        Portfolios = new();
        Funds = new();
    }

    public void Save(Stream fs)
    {
        byte[] headerBytes = UTF8Encoding.UTF8.GetBytes(Header);
        fs.Write(headerBytes, 0, headerBytes.Length);

        FileHelper.WriteInt(fs, Portfolios.Count);
        foreach (Portfolio p in Portfolios)
            p.Save(fs);

        FileHelper.WriteInt(fs, Funds.Count);
        foreach (Fund f in Funds)
            f.Save(fs);
    }

    public List<Fund> PortfolioFunds(int portfolioID)
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

    public List<FundPerformance> PortfolioPerformance(int portfolioID)
    {
        List<FundPerformance> fundPerformances = new();
        Portfolio portfolio = Portfolios.Find(p => p.ID == portfolioID);
        if (portfolio is null)
            return fundPerformances;

        foreach (FundFigures ff in portfolio.Funds)
        {
            Fund fund = Funds.Find(f => f.ID == ff.FundID);
            if (fund is not null)
                fundPerformances.Add(new FundPerformance { Fund = fund, AverageCost = ff.AverageCost });
        }
        return fundPerformances;
    }

    public int AddFund(string name, string morningstarID, string description = "")
    {
        Fund f = new(maxFundID, name, morningstarID, description);
        Funds.Add(f);
        maxFundID++;
        return maxFundID - 1;
    }

    public void UpdateFund(int fundID, string name, string description)
    {
        Fund f = Funds.Find(f => f.ID == fundID);
        if (f == null)
            return;

        f.Name = name;
        f.Description = description;
    }

    public void RemoveFund(int fundID)
    {
        Fund fund = Funds.Find(f => f.ID == fundID);

        if (fund is null)
            return;

        Funds.Remove(fund);

        foreach (Portfolio p in Portfolios)
            p.Funds.RemoveAll(f => f.FundID == fundID);
    }

    public void AddPortfolio(string name, string description = "")
    {
        Portfolio p = new(maxPortfolioID, name, new List<FundFigures>(), description);
        Portfolios.Add(p);
        maxPortfolioID++;
    }

    public void UpdatePortfolio(int portfolioID, string name, string description)
    {
        Portfolio p = Portfolios.Find(f => f.ID == portfolioID);
        if (p is null)
            return;

        p.Name = name;
        p.Description = description;
    }

    public void RemovePortfolio(int portfolioID)
    {
        Portfolio portfolio = Portfolios.Find(p => p.ID == portfolioID);

        if (portfolio is not null)
            Portfolios.Remove(portfolio);
    }

    public void AddFundToPortfolio(int portfolioID, int fundID, double averageCost)
    {
        Portfolio p = Portfolios.Find(p => p.ID == portfolioID);

        if (p is null)
            return;

        p.Funds.Add(new FundFigures(fundID, averageCost));
    }

    public void Load(Stream fs)
    {
        ReadAndCheckHeader(fs);
        List<Portfolio> portfolios = ReadPortfolios(fs);
        List<Fund> funds = ReadFunds(fs);
        CalculateMaxPortfolioID(portfolios);
        CalculateMaxFundID(funds);

        Portfolios = portfolios;
        Funds = funds;
    }

    private List<Fund> ReadFunds(Stream fs)
    {
        List<Fund> funds = new();
        int fundsCount = FileHelper.ReadInt(fs);
        for (int i = 0; i < fundsCount; i++)
            funds.Add(new(fs));

        return funds;
    }

    private List<Portfolio> ReadPortfolios(Stream fs)
    {
        List<Portfolio> portfolios = new();
        int portfoliosCount = FileHelper.ReadInt(fs);
        for (int i = 0; i < portfoliosCount; i++)
            portfolios.Add(new(fs));

        return portfolios;
    }

    private void ReadAndCheckHeader(Stream fs)
    {
        byte[] headerBytes = UTF8Encoding.UTF8.GetBytes(Header);
        fs.Read(headerBytes, 0, headerBytes.Length);
        byte[] checkHeaderBytes = UTF8Encoding.UTF8.GetBytes(Header);

        if (!checkHeaderBytes.SequenceEqual(headerBytes))
            throw new Exception("Erreur de header");
    }

    private void CalculateMaxFundID(List<Fund> funds)
    {
        if (funds.Count == 0)
        {
            maxFundID = 1;
            return;
        }
        maxFundID = Funds.Max(f => f.ID) + 1;
    }

    private void CalculateMaxPortfolioID(List<Portfolio> portfolios)
    {
        if (portfolios.Count == 0)
        {
            maxPortfolioID = 1;
            return;
        }
        maxPortfolioID = Portfolios.Max(p => p.ID) + 1;
    }
}