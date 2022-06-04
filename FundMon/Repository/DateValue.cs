using System;
using System.IO;

namespace FundMon.Repository;

public class DateValue
{
    private readonly DateTime _date;
    private readonly double _value;

    public double Value { get => _value; }
    public DateTime Date { get => _date; }

    public DateValue(double value, DateTime date)
    {
        _value = value;
        _date = date;
    }
    public DateValue(Stream s)
    {
        _date = FileHelper.ReadDateTime(s);
        _value = FileHelper.ReadDouble(s);
    }

    public void Save(Stream s)
    {
        FileHelper.WriteDateTime(s, Date);
        FileHelper.WriteDouble(s, Value);
    }
}
