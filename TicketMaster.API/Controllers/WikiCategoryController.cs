using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using TicketMaster.API.Services;
using TicketMaster.API.Models;

namespace TicketMaster.API.Controllers
{
    public class WikiCategoryController
    {

        /// <summary>
        /// Gets all the categories for wikis.
        /// </summary>
        /// <returns></returns>
        [FunctionName("GetAllWikiCategories")]
        public static async Task<IActionResult> GetAllWikiCategories(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wiki/categories")]
             HttpRequest request,
            ILogger log)
        {
            log.LogInformation("GetAllWikiCategories Function");

            var collection = new DatabaseService<WikiCategory>().GetCollection();

            return new OkObjectResult(collection);
        }

        /// <summary>
        /// Create a wiki category
        /// </summary>
        /// <returns></returns>
        [FunctionName("CreateWikiCategory")]
        public static async Task<IActionResult> CreateWikiCategory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "wiki/categories")]
             HttpRequest request,
            ILogger log)
        {
            log.LogInformation("CreateWikiCategory Function");

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<WikiCategory>(requestBody);

            var dbAccess = new DatabaseService<WikiCategory>();
            await dbAccess.SaveItem(data);

            var responseMessage = $"Successfully added new wiki category by provided data.";

            return new OkObjectResult(responseMessage);
        }

        /// <summary>
        /// Update a specifc wiki category
        /// </summary>
        /// <param name="Id">Id of the wiki category</param>
        /// <returns></returns>
        [FunctionName("UpdateWikiCategory")]
        public static async Task<IActionResult> UpdateWikiCategory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "wiki/categories/{Id}")]
            HttpRequest request,
            ILogger log, string Id)
        {
            log.LogInformation("UpdateWikiCategory Function");

            var category = new DatabaseService<WikiCategory>().GetCollection().Where(x => x.Id == Id).FirstOrDefault();
            if (category == null)
            {
                return new NotFoundObjectResult($"Could not find the category with id {Id}");
            }

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<WikiCategory>(requestBody);
            data.Id = category.Id; //assign id, since it will be missing.

            var dbAccess = new DatabaseService<WikiCategory>();
            await dbAccess.UpdateItem(data);

            var responseMessage = $"Successfully updated wiki category by provided data.";

            return new OkObjectResult(responseMessage);
        }



    }
}
