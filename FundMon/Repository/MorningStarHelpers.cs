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
    public string? Name;
    public string? MorningStarID;
    public string? Category;
    public string? Place;
    public string? Abbreviation;
}

internal static class MorningStarHelpers
{
    private readonly static HttpClient _client = new();
    private readonly static NumberFormatInfo _numberFormat = new CultureInfo("en-US").NumberFormat;
    private readonly static string[] filteredCategories = { "PEA", "Fonds", "Actions" };

    public static async Task<List<DateValue>> GetHistoricalFromID(string MorningStarID, DateTime? beginDate = null, DateTime? endDate = null)
    {
        string endPattern = (endDate ?? DateTime.Now).ToString("yyyy-MM-dd");
        string beginPattern = (beginDate ?? new DateTime(1991, 11, 29)).ToString("yyyy-MM-dd");
        string url = $"https://tools.morningstar.fr/api/rest.svc/timeseries_price/ok91jeenoo?" +
            $"id={MorningStarID}&currencyId=EUR&idtype=Morningstar&frequency=daily&" +
            $"startDate={beginPattern}&endDate={endPattern}&outputType=JSON";
        List<DateValue> values = new();

        try
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            MorningStarPayloadRoot? root = JsonConvert.DeserializeObject<MorningStarPayloadRoot>(content);
            List<HistoryDetail>? historyDetails = root?.TimeSeries?.Security?[0].HistoryDetail;

            if (historyDetails is null)
                return values;

            foreach (HistoryDetail h in historyDetails)
            {
                if (h.EndDate is null || h.EndDate.Length < 10 || h.Value is null)
                {
                    throw new Exception("Erreur de format de réponse");
                }
                values.Add(new DateValue(double.Parse(h.Value, _numberFormat), new DateTime(int.Parse(h.EndDate.Substring(0, 4)),
                    int.Parse(h.EndDate.Substring(5, 2)),
                    int.Parse(h.EndDate.Substring(8, 2)))));
            }
        }
        catch (Exception e)
        {
            Config.Config.AddLog($"Erreur de récupération des données depuis Morningstar : {e}");
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
                        MorningStarID = fields[1].Substring(left, right - left),
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