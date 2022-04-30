using System;

namespace FundMon.Repository;

public class FundPerformance
{
    public Fund Fund { get; set; }
    public double AverageCost { get; set; }

    public double LastWeekValue
    {
        get
        {
            DateTime now = DateTime.Now;
            if (Fund is null || Fund.Historical is null || Fund.Historical.Count == 0)
                return double.NaN;
            DateValue exact = Fund.Historical.Find(f => (now - f.Date).Days == 7);
            if (exact.Date != default)
                return exact.Value;
            DateValue further = Fund.Historical.Find(f => (now - f.Date).Days == 8);
            if (further.Date != default)
                return further.Value;
            DateValue further2 = Fund.Historical.Find(f => (now - f.Date).Days == 9);
            if (further2.Date != default)
                return further2.Value;
            return double.NaN;
        }
    }
}
