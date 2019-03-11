using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace Stats_API
{
    public static class Function1
    {
        private static string _playerURL = "http://www.espn.com/nba/player/stats/_/id/";

        [FunctionName("PER")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string playerID = req.Query["playerId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            playerID = playerID ?? data?.name;

            return playerID != null
                ? (ActionResult)new OkObjectResult(FetchPER(playerID))
                : new BadRequestObjectResult("Please pass a playerId on the query string or in the request body");
        }

        private static double FetchPER(string WhatPlayerID)
        {
            var result = -1.1;
            var position = -1;

            var uri = new Uri(_playerURL + WhatPlayerID);
            var request = HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;

            var response = request.GetResponse();
            var sr = new StreamReader(response.GetResponseStream());
            var content = sr.ReadToEnd();
            response.Close();

            var document = new HtmlDocument();
            document.LoadHtml(content);

            var table = document.DocumentNode.SelectSingleNode("//table[contains(@class, 'header-stats')]");

            while (result <= 0)
            {
                for (var i = 0; i < table.ChildNodes.Count; i++)
                {
                    var child = table.ChildNodes[i];
                    if (position > -1)
                    {
                        var parsePER = double.TryParse(child.ChildNodes[position].InnerText, out result);
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

            return result;
        }
    }
}
