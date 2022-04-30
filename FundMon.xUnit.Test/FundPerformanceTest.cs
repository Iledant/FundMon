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
            FundPerformance fundPerformance = new FundPerformance{Fund = fund, AverageCost = 150.0};
            Assert.Equal(double.NaN, fundPerformance.LastWeekValue);
            fund.Historical.Add(new DateValue { Date = now.AddDays(-9), Value = 123.5 });
            Assert.Equal(123.5, fundPerformance.LastWeekValue);
            fund.Historical.Add(new DateValue { Date = now.AddDays(-8), Value = 124.5 });
            Assert.Equal(124.5, fundPerformance.LastWeekValue);
            fund.Historical.Add(new DateValue { Date = now.AddDays(-7), Value = 125.5 });
            Assert.Equal(125.5, fundPerformance.LastWeekValue);
        }
    }
}
