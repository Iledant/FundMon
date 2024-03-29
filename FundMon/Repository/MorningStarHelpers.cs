﻿#nullable enable

using FundMon.Config;
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
    public string? Name;
    public string? MorningStarID;
    public string? Category;
    public string? Place;
    public string? Abbreviation;
}

public static class MorningStarHelpers
{
    private readonly static HttpClient _client = new();
    private readonly static NumberFormatInfo _numberFormat = new CultureInfo("en-US").NumberFormat;
    private readonly static string[] filteredCategories = { "PEA", "Fonds", "Actions", "ETFs" };
    private readonly static CultureInfo _usCultureInfo = new("en-US");

    public static async Task<List<DateValue>> GetCompactHistoricalFromID(string MorningStarID, DateTime? beginDate = null, DateTime? endDate = null)
    {
        string endPattern = (endDate ?? DateTime.Now).ToString("yyyy-MM-dd");
        string beginPattern = (beginDate ?? new DateTime(1991, 11, 29)).ToString("yyyy-MM-dd");
        string url = $"https://tools.morningstar.fr/api/rest.svc/timeseries_price/ok91jeenoo?" +
            $"id={MorningStarID}%5D22%5D1%5D&currencyId=EUR&idtype=Morningstar&frequency=daily&" +
            $"startDate={beginPattern}&endDate={endPattern}&outputType=COMPACTJSON";
        List<DateValue> values = new();
        char[] leadingCharToTrim = { ',', '[' };
        try
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            string innerList = content[1..^1]; // trim external square brackets
            string[] dateValues = innerList.Split(']');

            for (int i = 0; i < dateValues.Length-1; i++)
            {
                string[] dateAndValue = dateValues[i].Trim(leadingCharToTrim).Split(',');

                if (dateAndValue.Length != 2)
                    throw new Exception("Erreur de format de réponse");
                if (!double.TryParse(dateAndValue[1], NumberStyles.Number,_usCultureInfo, out double value))
                    throw new Exception("Erreur de format de réponse");
                if (!long.TryParse(dateAndValue[0], out long dateInMilliseconds))
                    throw new Exception("Erreur de format de réponse");

                values.Add(new DateValue(value, new DateTime(1970, 1, 1).AddMilliseconds(dateInMilliseconds)));
            }
        }
        catch (Exception e)
        {
            AppConfig.AddLog($"Erreur de récupération des données depuis Morningstar : {e}","Erreur");
        }
        return values;
    }

    private static List<MorningstarResponseLine> ParseMorningstarResponse(string content)
    {
        char[] charToStrim = { '\n', '\r' };
        string[] lines = content.Split("\n");
        List<MorningstarResponseLine> pickStocks = new();
        foreach (string line in lines)
        {
            string[] fields = line.Split('|');
            if (fields.Length < 6)
                continue;

            int left = fields[1].IndexOf("\"i\":\"") + 5;
            int right = fields[1].IndexOf("\",\"");
            if (left == -1 || right == -1 || right <= left)
                continue;

            string category = fields[5].Trim(charToStrim);

            for (int i = 0; i < filteredCategories.Length; i++)
            {
                if (category == filteredCategories[i])
                {
                    pickStocks.Add(new MorningstarResponseLine
                    {
                        Name = fields[0].Trim(charToStrim),
                        MorningStarID = fields[1][left..right],
                        Category = category,
                        Place = fields[4].Trim(charToStrim),
                        Abbreviation = fields[3].Trim(charToStrim)
                    });
                    break;
                }
            }
        }
        return pickStocks;
    }

    public static async Task<List<MorningstarResponseLine>> FetchFunds(string pattern)
    {
        string url = "https://www.morningstar.fr/fr/util/SecuritySearch.ashx?source=nav&moduleId=6&ifIncludeAds=False&usrtType=v";
        FormUrlEncodedContent formContent = new(new[]
        {
            new KeyValuePair<string, string>("q", pattern),
            new KeyValuePair<string, string>("limit", "100")
        });

        try
        {

            HttpResponseMessage response = await _client.PostAsync(url, formContent);
            string content = await response.Content.ReadAsStringAsync();
            return ParseMorningstarResponse(content);
        }
        catch (Exception)
        {
            return new List<MorningstarResponseLine>();
        }
    }
}