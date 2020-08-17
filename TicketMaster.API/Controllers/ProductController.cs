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
    public static class ProductController
    {
        [FunctionName("GetProductById")]
        public static async Task<IActionResult> GetProductById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "product/{id}")]
            HttpRequest request, ILogger log, string id)
        {
            log.LogInformation("GetProductById Function");

            var collection = new DatabaseService<Product>().GetCollection();
            var requestedItem = collection.Where(x => x.Id == id);

            return new OkObjectResult(requestedItem);
        }

        [FunctionName("GetAllProducts")]
        public static async Task<IActionResult> GetAllProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "product")]
            HttpRequest request, ILogger log)
        {
            log.LogInformation("GetAllProducts Function");

            var collection = new DatabaseService<Product>().GetCollection();

            return new OkObjectResult(collection);
        }

        [FunctionName("CreateProduct")]
        public static async Task<IActionResult> CreateProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "product")]
            HttpRequest request, ILogger log)
        {
            log.LogInformation("CreateProduct Function");

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Product>(requestBody);

            var databaseAccess = new DatabaseService<Product>();
            await databaseAccess.SaveItem(data);

            var responseMessage = "Succesfully created product";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("DeleteProductById")]
        public static async Task<IActionResult> DeleteProductById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "product/{id}")]
            HttpRequest request, ILogger log, string id)
        {
            log.LogInformation("DeleteProductById Function");

            var databaseAccess = new DatabaseService<Product>();
            await databaseAccess.DeleteItem(id);

            var responseMessage = "Succesfully deleted product";

            return new OkObjectResult(responseMessage);
        }
    }
}
