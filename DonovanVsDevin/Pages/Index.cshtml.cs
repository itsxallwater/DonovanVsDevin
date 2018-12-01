using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using HtmlAgilityPack;

namespace DonovanVsDevin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMemoryCache _cache;

        public string Donovan_URL = "http://www.espn.com/nba/player/stats/_/id/3908809/donovan-mitchell";
        public string Devin_URL = "http://www.espn.com/nba/player/stats/_/id/3136193/devin-booker";

        public string Donovan_Class { get; set; }
        public string Devin_Class { get; set; }

        public double Donovan_PER { get; set; }
        public double Devin_PER { get; set; }

        public IndexModel(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnGet()
        {
            // Load Donovan's PER
            double donovanPER;
            if (_cache.TryGetValue("donovanPER", out donovanPER))
                Donovan_PER = donovanPER;
            else
            {
                Donovan_PER = LoadDonovan();
                _cache.Set<double>("donovanPER", Donovan_PER, new DateTimeOffset(DateTime.Today.AddHours(6)));
            }

            // Load Devin's PER
            double devinPER;
            if (_cache.TryGetValue("devinPER", out devinPER))
                Devin_PER = devinPER;
            else
            {
                Devin_PER = LoadDevin();
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

        public double LoadDonovan()
        {
            double donovanPER = -1.1;
            var position = -1;

            var uriDonovan = new Uri(Donovan_URL);
            var reqDonovan = HttpWebRequest.Create(uriDonovan);
            reqDonovan.Method = WebRequestMethods.Http.Get;

            var respDonovan = reqDonovan.GetResponse();
            var readDonovan = new StreamReader(respDonovan.GetResponseStream());
            var contentDonovan = readDonovan.ReadToEnd();
            respDonovan.Close();

            var donovanDoc = new HtmlDocument();
            donovanDoc.LoadHtml(contentDonovan);

            var donovanTable = donovanDoc.DocumentNode.SelectSingleNode("//table[contains(@class, 'header-stats')]");

            while (donovanPER <= 0)
            {
                for (var i = 0; i < donovanTable.ChildNodes.Count; i++)
                {
                    var child = donovanTable.ChildNodes[i];
                    if (position > -1)
                    {
                        var donovanParse = double.TryParse(child.ChildNodes[position].InnerText, out donovanPER);
                        position = -1;
                        break;
                    }
                    else
                    {

                        for (var j = 0; j < child.ChildNodes.Count; j++)
                        {
                            var grandChild = child.ChildNodes[j];

                            if (grandChild.InnerHtml.Equals("PER"))
                            {
                                position = j;
                                break;
                            }
                        }
                    }
                }
            }

            return donovanPER;
        }

        public double LoadDevin()
        {
            double devinPER = -1.1;
            var position = -1;

            var uriDevin = new Uri(Devin_URL);
            var reqDevin = HttpWebRequest.Create(uriDevin);
            reqDevin.Method = WebRequestMethods.Http.Get;

            var respDevin = reqDevin.GetResponse();
            var readDevin = new StreamReader(respDevin.GetResponseStream());
            var contentDevin = readDevin.ReadToEnd();
            respDevin.Close();

            var devinDoc = new HtmlDocument();
            devinDoc.LoadHtml(contentDevin);

            var devinTable = devinDoc.DocumentNode.SelectSingleNode("//table[contains(@class, 'header-stats')]");

            while (devinPER <= 0)
            {
                for (var i = 0; i < devinTable.ChildNodes.Count; i++)
                {
                    var child = devinTable.ChildNodes[i];
                    if (position > -1)
                    {
                        var devinParse = double.TryParse(child.ChildNodes[position].InnerText, out devinPER);
                        break;
                    }
                    else
                    {

                        for (var j = 0; j < child.ChildNodes.Count; j++)
                        {
                            var grandChild = child.ChildNodes[j];

                            if (grandChild.InnerHtml.Equals("PER"))
                            {
                                position = j;
                                break;
                            }
                        }
                    }
                }
            }

            return devinPER;
        }
    }
}
