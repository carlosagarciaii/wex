using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TransactAPI.Models;

public class CurrencyService : IAsyncDisposable
{
    private readonly HttpClient _httpClient;

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            _httpClient.CancelPendingRequests();
        }
        finally
        {
            await Task.CompletedTask;
        }
    }

    public async Task<double?> GetExchangeRateAsync(string currency, DateTime targetDate)
    {
        try
        {
            ///////////////////////////////////////////////////////////////
            // Widen Gap to get values up to 1 year prior
            //      (allows for irregular reporting times)
            ///////////////////////////////////////////////////////////////
            DateTime oneYearPrior = targetDate.AddYears(-1);
            string oneYearPriorStr = oneYearPrior.ToString("yyyy-MM-dd");
            string targetDateStr = targetDate.ToString("yyyy-MM-dd");



            ///////////////////////////////////////////////////////////////
            /// API Request
            ///////////////////////////////////////////////////////////////
            string apiUrl = $"https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange?fields=country_currency_desc,exchange_rate,record_date&filter=country_currency_desc:in:({currency}),record_date:gte:{oneYearPriorStr}";

            var response = await _httpClient.GetFromJsonAsync<ExchangeRateAPIResponseModel>(apiUrl);

            if (response?.Data == null || response.Data.Count == 0)
            {
                return null;
            }


            ///////////////////////////////////////////////////////////////
            ///  Filter Results
            ///////////////////////////////////////////////////////////////
            var currencyData = response.Data
                .Where(item =>
                    item.CountryCurrencyDesc
                        .Equals(currency, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (currencyData.Count == 0)
            {
                return null;
            }


            ///////////////////////////////////////////////////////////////
            /// Get Closest Date Match
            ///////////////////////////////////////////////////////////////
            var eligibleRecords = currencyData
                .Where(item => DateTime.Parse(item.RecordDate) <= targetDate)
                .OrderByDescending(item => DateTime.Parse(item.RecordDate))
                .ToList();

            if (eligibleRecords.Count == 0)
            {
                eligibleRecords = currencyData
                    .OrderBy(item => DateTime.Parse(item.RecordDate))
                    .ToList();
            }

            var closestRecord = eligibleRecords.FirstOrDefault();
            if (closestRecord == null)
            {
                return null;
            }


            ///////////////////////////////////////////////////////////////
            /// REsults
            ///////////////////////////////////////////////////////////////
            if (double.TryParse(closestRecord.ExchangeRate, out double exchangeRate))
            {
                return exchangeRate;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error harvesting exchange rates: {ex.Message}");
            return null;
        }
    }


    public async Task<(double? Rate, string RecordDate)> GetExchangeRateWithDetailsAsync(string currency, DateTime targetDate)
    {
        var rate = await GetExchangeRateAsync(currency, targetDate);

        if (rate.HasValue)
        {
            DateTime oneYearPrior = targetDate.AddYears(-1);
            string oneYearPriorStr = oneYearPrior.ToString("yyyy-MM-dd");

            string apiUrl = $"https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange?fields=country_currency_desc,exchange_rate,record_date&filter=country_currency_desc:in:({currency}),record_date:gte:{oneYearPriorStr}&page[size]=100";

            var response = await _httpClient.GetFromJsonAsync<ExchangeRateAPIResponseModel>(apiUrl);

            var record = response?.Data?
                .Where(item => item.CountryCurrencyDesc.Equals(currency, StringComparison.OrdinalIgnoreCase))
                .Where(item => DateTime.Parse(item.RecordDate) <= targetDate)
                .OrderByDescending(item => DateTime.Parse(item.RecordDate))
                .FirstOrDefault();

            return (rate, record?.RecordDate);
        }

        return (null, null);
    }
}
