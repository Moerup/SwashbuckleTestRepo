using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver.Linq;
using TicketMaster.API.Services;
using TicketMaster.API.Models;

namespace TicketMaster.API.Controllers
{
    public static class CommentController
    {
        [FunctionName("GetCommentsOnConfigurationItemId")]
        public static async Task<IActionResult> GetCommentsOnConfigurationItemId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "comment/{configurationitemid}")]
            HttpRequest request, ILogger log, string configurationitemid)
        {
            log.LogInformation("GetCommentsOnConfigurationItem Triggered");

            var collection = new DatabaseService<Comment>().GetCollection();
            var requestedItem = collection.Where(x => x.ConfigurationItemId == configurationitemid);

            return new OkObjectResult(requestedItem);
        }

        [FunctionName("CreateComment")]
        public static async Task<IActionResult> CreateComment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "comment")]
            HttpRequest request, ILogger log)
        {
            log.LogInformation("CreateComment Triggered");

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Comment>(requestBody);

            var databaseAccess = new DatabaseService<Comment>();
            await databaseAccess.SaveItem(data);

            var responseMessage = "Succesfully created comment";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("DeleteCommentById")]
        public static async Task<IActionResult> DeleteCommentById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "comment/{id}")]
            HttpRequest request, ILogger log, string id)
        {
            log.LogInformation("DeleteCommentById Triggered");

            var databaseAccess = new DatabaseService<Product>();
            await databaseAccess.DeleteItem(id);

            var responseMessage = "Succesfully deleted comment";

            return new OkObjectResult(responseMessage);
        }
    }
}
