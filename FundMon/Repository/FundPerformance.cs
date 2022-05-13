using System;

namespace FundMon.Repository;

public class FundPerformance
{
    public Fund Fund { get; set; }
    public double AverageCost { get; set; }

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

    public double Evolution => LastValue - AverageCost;

    public double LastWeekValue { get => FetchValue(7); }

    public double LastMonthValue { get => FetchValue(31); }

    public double LastValue
    {
        get
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
}
