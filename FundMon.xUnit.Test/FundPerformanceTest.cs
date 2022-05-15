using FundMon.Repository;
using System;
using Xunit;

namespace FundMon.xUnit.Test
{
    public class FundPerformanceTest
    {
        [Fact]
        public void LastWeekValueTest()
        {
            Fund fund = new(1, "Fond performance","Morningstar ID performance");
            DateTime now = DateTime.Now;
            FundPerformance fundPerformance = new FundPerformance(fund, 150.0);
            Assert.Equal(double.NaN, fundPerformance.LastWeekValue);
            fund.Historical.Add(new DateValue(123.5, now.AddDays(-9)));
            fund.HistoricalChanged?.Invoke();
            Assert.Equal(123.5, fundPerformance.LastWeekValue);
            fund.Historical.Add(new DateValue(124.5, now.AddDays(-8)));
            fund.HistoricalChanged?.Invoke();
            Assert.Equal(124.5, fundPerformance.LastWeekValue);
            fund.Historical.Add(new DateValue(125.5, now.AddDays(-7)));
            fund.HistoricalChanged?.Invoke();
            Assert.Equal(125.5, fundPerformance.LastWeekValue);
        }
    }
}
