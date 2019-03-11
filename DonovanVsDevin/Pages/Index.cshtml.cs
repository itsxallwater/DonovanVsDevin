using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace DonovanVsDevin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMemoryCache _cache;
        private static IConfiguration _config;

        private static HttpClient _httpClient = new HttpClient();

        private static string _perAPI = "https://donovanvsdevinstatsapi.azurewebsites.net/api/PER";
        private static string _DonovanID = "3908809";
        private static string _DevinID = "3136193";

        public string Donovan_Class { get; set; }
        public string Devin_Class { get; set; }

        public double Donovan_PER { get; set; }
        public double Devin_PER { get; set; }

        public IndexModel(IMemoryCache cache, IConfiguration config)
        {
            _cache = cache;
            _config = config;
        }

        public void OnGet()
        {
            // Load Donovan's PER
            double donovanPER;
            if (_cache.TryGetValue("donovanPER", out donovanPER))
                Donovan_PER = donovanPER;
            else
            {
                Donovan_PER = LoadPER(_DonovanID).Result;
                _cache.Set<double>("donovanPER", Donovan_PER, new DateTimeOffset(DateTime.Today.AddHours(6)));
            }

            // Load Devin's PER
            double devinPER;
            if (_cache.TryGetValue("devinPER", out devinPER))
                Devin_PER = devinPER;
            else
            {
                Devin_PER = LoadPER(_DevinID).Result;
                _cache.Set<double>("devinPER", Devin_PER, new DateTimeOffset(DateTime.Today.AddHours(6)));
            }

            // Decide on Display
            if (Donovan_PER > Devin_PER)
            {
                Donovan_Class = "ribbon-winner";
                Devin_Class = "ribbon-loser";
            }
            else
            {
                Donovan_Class = "ribbon-loser";
                Devin_Class = "ribbon-winner";
            }
        }

        public static async Task<double> LoadPER(string WhatID)
        {
            var result = -1.1;
            var response = await _httpClient.GetAsync(_perAPI + "?code=" + _config["perCode"] + "&playerId=" + WhatID);
            var content = await response.Content.ReadAsStringAsync();

            var parseResponse = double.TryParse(content, out result);

            return result;
        }
    }
}
