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
    class FieldController
    {
        [FunctionName("GetFieldById")]
        public static async Task<IActionResult> GetFieldById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "field/{id}")]
            HttpRequest request, ILogger log, string id)
        {
            log.LogInformation("GetFieldById Triggered");

            var collection = new DatabaseService<Field>().GetCollection();
            var requestedItem = collection.Where(x => x.Id == id);

            return new OkObjectResult(requestedItem);
        }

        [FunctionName("GetAllFields")]
        public static async Task<IActionResult> GetAllFields(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "field")]
            HttpRequest request, ILogger log)
        {
            log.LogInformation("GetAllProducts Function");

            var collection = new DatabaseService<Field>().GetCollection();

            return new OkObjectResult(collection);
        }

        [FunctionName("CreateField")]
        public static async Task<IActionResult> CreateField(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "field")]
            HttpRequest request, ILogger log)
        {
            log.LogInformation("CreateFields Triggered");

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Field>(requestBody);

            var databaseAccess = new DatabaseService<Field>();
            await databaseAccess.SaveItem(data);

            var responseMessage = "Succesfully created field";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("DeleteFieldById")]
        public static async Task<IActionResult> DeleteFieldById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "field/{id}")]
            HttpRequest request, ILogger log, string id)
        {
            log.LogInformation("DeleteFieldById Triggered");

            var databaseAccess = new DatabaseService<Field>();
            await databaseAccess.DeleteItem(id);

            var responseMessage = "Succesfully deleted field";

            return new OkObjectResult(responseMessage);
        }
    }
}
