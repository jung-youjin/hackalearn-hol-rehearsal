using System;
using System.Net.Http;
using System.Threading.Tasks;

using Api.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Api
{
    public static class CmsHttpTrigger
    {
        [FunctionName("WordpressHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "GET", Route = "posts")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var blogUri = Environment.GetEnvironmentVariable("BLOG_URI");
            var requestUri = $"https://public-api.wordpress.com/rest/v1.1/sites/{blogUri}/posts/";

            var posts = default(PostCollection);
            using (var http = new HttpClient())
            {
                var response = await http.GetStringAsync(requestUri);
                posts = JsonConvert.DeserializeObject<PostCollection>(response);
            }

            return new OkObjectResult(posts);
        }
    }
}