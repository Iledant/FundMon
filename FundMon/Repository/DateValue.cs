using System;
using System.IO;

namespace FundMon.Repository;

public class DateValue
{
    public double Value { get; init; }
    public DateTime Date { get; init; }

    public DateValue(double value, DateTime date)
    {
        Value = value;
        Date = date;
    }
    public DateValue(Stream s)
    {
        Date = FileHelper.ReadDateTime(s);
        Value = FileHelper.ReadDouble(s);
    }

    public void Save(Stream s)
    {
        FileHelper.WriteDateTime(s, Date);
        FileHelper.WriteDouble(s, Value);
    }
}
