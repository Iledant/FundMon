using CommunityToolkit.Mvvm.ComponentModel;
using FundMon.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace FundMon.Repository;

public partial class FundPerformance : ObservableObject, IEditableObject
{
    private double averageCost;
    [ObservableProperty]
    private Fund fund;
    private bool inTransaction = false;
    private double backupAverageCost;
    private double evolution = double.NaN;
    private double lastWeekValue = double.NaN;
    private double lastMonthValue = double.NaN;
    private double lastValue = double.NaN;

    public double AverageCost
    {
        get => averageCost;
        set
        {
            averageCost = value;
            ComputeValues();
            SetProperty(ref averageCost, value);
        }
    }

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
        fund.Historical.CollectionChanged += Historical_CollectionChanged;
        AverageCost = averageCost;
        ComputeValues();
    }

    public FundPerformance(Stream s, List<Fund> fundCollection)
    {
        int ID = FileHelper.ReadInt(s);
        for (int i = 0; i < fundCollection.Count; i++)
        {
            if (fundCollection[i].ID == ID)
            {
                Fund = fundCollection[i];
                break;
            }
        }
        AverageCost = FileHelper.ReadDouble(s);
    }

    public void Save(Stream s)
    {
        FileHelper.WriteInt(s, Fund.ID);
        FileHelper.WriteDouble(s, AverageCost);
    }

    private void Historical_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        ComputeValues();
    }

    private void ComputeValues()
    {
        lastWeekValue = FetchValue(7);
        lastMonthValue = FetchValue(31);
        lastValue = FetchLastValue();
        evolution = double.IsNaN(lastValue) ? double.NaN : lastValue - AverageCost;
        OnPropertyChanged(nameof(Evolution));
        OnPropertyChanged(nameof(LastWeekValue));
        OnPropertyChanged(nameof(LastMonthValue));
        OnPropertyChanged(nameof(LastValue));
    }

    private double FetchValue(int days)
    {
        DateTime now = DateTime.Now;
        if (Fund is null || Fund.Historical is null || Fund.Historical.Count == 0)
            return double.NaN;
        double exact = double.NaN;
        double further = double.NaN;
        double further2 = double.NaN;

        for (int i = 0; i < Fund.Historical.Count; i++)
        {
            var value = (now - Fund.Historical[i].Date).Days - days;

            if (value == 0)
            {
                exact = Fund.Historical[i].Value;
                break;
            }
            if (value == 1)
            {
                further = Fund.Historical[i].Value;
            }
            if (value == 2)
            {
                further2 = Fund.Historical[i].Value;
            }
        }
        if (!double.IsNaN(exact))
            return exact;
        if (!double.IsNaN(further))
            return further;
        return further2;
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

    public void BeginEdit()
    {
        if (!inTransaction)
        {
            inTransaction = true;
            backupAverageCost = averageCost;
        }
    }

    public void CancelEdit()
    {
        if (inTransaction)
        {
            inTransaction = false;
            averageCost = backupAverageCost;
        }
    }

    public void EndEdit()
    {
        if (inTransaction)
        {
            inTransaction = false;
        }
    }
}
