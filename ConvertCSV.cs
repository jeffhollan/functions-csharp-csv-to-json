using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Hollan.Function
{
    public static class ConvertCSV
    {
        [FunctionName("ConvertCSV")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var csv = new List<string[]>();
            foreach(var line in requestBody.Split("\n"))
            {
                csv.Add(line.Split(","));
            }

            var headers = csv[0];
            csv.RemoveRange(0, 1);

            JArray jsonResponse = new JArray();
            foreach(var row in csv)
            {
                var rowObject = new JObject();
                for(int index = 0; index < row.Length; index++)
                {
                    rowObject.Add(headers[index], row[index]);
                }
                jsonResponse.Add(rowObject);
            }
            return new OkObjectResult(jsonResponse);
        }
    }
}
