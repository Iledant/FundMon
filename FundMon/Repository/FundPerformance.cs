using FundMon.Helpers;
using FundMon.ViewModel;
using System;
using System.Globalization;

namespace FundMon.Repository;

public class FundPerformance : Bindable
{
    private double averageCost;
    public Fund Fund { get; init; }
    public double AverageCost {
        get => averageCost;
        set
        {
            averageCost = value;
            OnPropertyChanged();
        }
    }
    static readonly CultureInfo culture = new("fr-FR");

    private double evolution = double.NaN;
    private double lastWeekValue = double.NaN;
    private double lastMonthValue = double.NaN;
    private double lastValue = double.NaN;

    public double Evolution => evolution;
    public string PercentageEvolution => Formatter.Percentage(evolution);
    public string EuroEvolution => Formatter.Currency(evolution);
    public string GlyphEvolution => Formatter.EvolutionGlyph(evolution);
    public double LastValue => lastValue;
    public string EuroLastValue => Formatter.Currency(lastValue);
    public double LastWeekValue => lastWeekValue;
    public string EuroLastWeekValue => Formatter.Currency(lastWeekValue);
    public double LastMonthValue => lastMonthValue;
    public string EuroLastMonthValue => Formatter.Currency(lastMonthValue);
    public string EuroAverageCost => Formatter.Currency(AverageCost);

    public FundPerformance(Fund fund, double averageCost)
    {
        Fund = fund;
        AverageCost = averageCost;
        Fund.HistoricalChanged = HistoricalChanged;
        HistoricalChanged();
    }

    public void HistoricalChanged()
    {
        lastWeekValue = FetchValue(7);
        lastMonthValue = FetchValue(31);
        lastValue = FetchLastValue();
        evolution = double.IsNaN(lastValue) ? double.NaN : lastValue - AverageCost;
    }

    private double FetchValue(int days)
    {
        DateTime now = DateTime.Now;
        if (Fund is null || Fund.Historical is null || Fund.Historical.Count == 0)
            return double.NaN;
        DateValue exact = Fund.Historical.Find(f => (now - f.Date).Days == days);
        if (exact is not null)
            return exact.Value;
        DateValue further = Fund.Historical.Find(f => (now - f.Date).Days == days + 1);
        if (further is not null)
            return further.Value;
        DateValue further2 = Fund.Historical.Find(f => (now - f.Date).Days == days + 2);
        if (further2 is not null)
            return further2.Value;
        return double.NaN;
    }

    private double FetchLastValue()
    {
        if (Fund.Historical is null || Fund.Historical.Count == 0)
            return double.NaN;
        double value = Fund.Historical[0].Value;
        DateTime dateTime = Fund.Historical[0].Date;
        for (int i = 1; i < Fund.Historical.Count; i++)
            if (Fund.Historical[i].Date > dateTime)
            {
                value = Fund.Historical[i].Value;
                dateTime = Fund.Historical[i].Date;
            }
        return value;
    }
}
