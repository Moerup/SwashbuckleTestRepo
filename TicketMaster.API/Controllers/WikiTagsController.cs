using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TicketMaster.API.Models;
using TicketMaster.API.Services;

namespace TicketMaster.API.Controllers
{
    public class WikiTagsController
    {
        /// <summary>
        /// Returns all wiki tags.
        /// </summary>
        /// <returns></returns>
        [FunctionName("GetAllWikiTags")]
        public static async Task<IActionResult> GetAllWikiTags(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wiki/Tags")]
            HttpRequest request,
            ILogger log)
        {
            log.LogInformation("GetAllWikiTags Function");

            var collection = new DatabaseService<WikiTag>().GetCollection();

            return new OkObjectResult(collection);
        }

        [FunctionName("CreateWikiTag")]
        public static async Task<IActionResult> CreateWikiTag(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "wiki/Tags")]
             HttpRequest request,
            ILogger log)
        {
            log.LogInformation("CreateWikiTag Function");

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<WikiTag>(requestBody);

            var dbAccess = new DatabaseService<WikiTag>();
            await dbAccess.SaveItem(data);

            var responseMessage = $"Successfully added new wiki tag by provided data.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("DeleteWikiTag")]
        public static async Task<IActionResult> DeleteWikiTag(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "wiki/Tags/{Id}")]
             HttpRequest request,
            ILogger log, string Id)
        {
            log.LogInformation("DeleteWikiTag Function");

            var dbAccess = new DatabaseService<WikiTag>();
            await dbAccess.DeleteItem(Id);

            var responseMessage = $"Successfully deleted wiki tag by provided data.";

            return new OkObjectResult(responseMessage);
        }
    }
}
