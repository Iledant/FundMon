#nullable enable

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace FundMon.Repository;

public class HistoryDetail
{
    public string? EndDate { get; set; }
    public string? Value { get; set; }
}

public class Security
{
    public List<HistoryDetail>? HistoryDetail { get; set; }
    public string? Id { get; set; }
}

public class TimeSeries
{
    public List<Security>? Security { get; set; }
}
public class MorningStarPayloadRoot
{
    public TimeSeries? TimeSeries { get; set; }
}

public class MorningstarResponseLine
{
    public string Name;
    public string MorningStarID;
    public string Category;
    public string Place;
    public string Abbreviation;
}

internal static class MorningStarHelpers
{
    private readonly static HttpClient _client = new();
    private readonly static NumberFormatInfo _numberFormat = new CultureInfo("en-US").NumberFormat;

    public static async Task<List<DateValue>> GetHistoricalFromID(string MorningStarID, DateTime? beginDate = null, DateTime? endDate = null)
    {
        string endPattern = (endDate ?? DateTime.Now).ToString("yyyy-MM-dd");
        string beginPattern = (beginDate ?? new DateTime(1991, 11, 29)).ToString("yyyy-MM-dd");
        string url = $"https://tools.morningstar.fr/api/rest.svc/timeseries_price/ok91jeenoo?" +
            $"id={MorningStarID}&currencyId=EUR&idtype=Morningstar&frequency=daily&" +
            $"startDate={beginPattern}&endDate={endPattern}&outputType=JSON";

        HttpResponseMessage response = await _client.GetAsync(url);
        string content = await response.Content.ReadAsStringAsync();
        MorningStarPayloadRoot? root = JsonConvert.DeserializeObject<MorningStarPayloadRoot>(content);
        List<HistoryDetail>? historyDetails = root?.TimeSeries?.Security?[0].HistoryDetail;
        List<DateValue> values = new();

        if (historyDetails is null)
        {
            return values;
        }

        foreach (HistoryDetail h in historyDetails)
        {
            if (h.EndDate is null || h.EndDate.Length < 10 || h.Value is null)
            {
                throw new Exception("Erreur de format de réponse");
            }
            values.Add(new DateValue(new DateTime(int.Parse(h.EndDate.Substring(0, 4)),
                int.Parse(h.EndDate.Substring(5, 2)),
                int.Parse(h.EndDate.Substring(8, 2))), double.Parse(h.Value, _numberFormat)));
        }
        return values;
    }

    private static List<MorningstarResponseLine> ParseMorningstarResponse(string content)
    {
        string[] lines = content.Split("\n");
        List<MorningstarResponseLine> pickStocks = new();
        foreach (string line in lines)
        {
            string[] fields = line.Split('|');
            if (fields.Length < 6)
            {
                continue;
            }
            int left = fields[1].IndexOf("\"i\":\"") + 5;
            int right = fields[1].IndexOf("\",\"");
            if (left == -1 || right == -1 || right <= left)
            {
                continue;
            }
            pickStocks.Add(new MorningstarResponseLine
            {
                Name = fields[0],
                MorningStarID = fields[1].Substring(left, right - left),
                Category = fields[5],
                Place = fields[4],
                Abbreviation = fields[3]
            });
        }
        return pickStocks;
    }

    public static async Task<List<MorningstarResponseLine>> FetchFunds(string pattern)
    {
        string url = $"https://www.morningstar.fr/fr/util/SecuritySearch.ashx?" +
            $"ifIncludeAds=False&q={pattern}&limit=100";

        try
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            return ParseMorningstarResponse(content);
        }
        catch (Exception)
        {
            return new List<MorningstarResponseLine>();
        }
    }
}