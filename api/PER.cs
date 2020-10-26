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

            var uri = new Uri(_playerURL + WhatPlayerID);
            var request = HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;

            var response = request.GetResponse();
            var sr = new StreamReader(response.GetResponseStream());
            var content = sr.ReadToEnd();
            response.Close();

            var document = new HtmlDocument();
            document.LoadHtml(content);

            var divPER = document.DocumentNode.SelectSingleNode("//div[contains(@aria-label, 'Player Efficiency Rating')]");
            var parsePER = double.TryParse(divPER.ParentNode.ChildNodes[1].InnerText, out result);

            return result;
        }
    }
}
