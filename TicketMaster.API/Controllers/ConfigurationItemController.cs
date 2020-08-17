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
using TicketMaster.API.Interfaces;
using System;

namespace TicketMaster.API.Controllers
{
    public class ConfigurationItemController
    {
        private readonly IAuthenticationService _authenticationService;

        public ConfigurationItemController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [FunctionName("GetConfigurationItem")]
        public async Task<IActionResult> GetConfigurationItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "configurationitem/{id}")]
            HttpRequest request, ILogger log, string id)
        {
            log.LogInformation("GetConfigurationItem Function");

            var collection = new DatabaseService<ConfigurationItem>().GetCollection();
            var requestedItem = collection.Where(x => x.Id == id);

            return new OkObjectResult(requestedItem);
        }

        [FunctionName("GetAllConfigurationItem")]
        public async Task<IActionResult> GetAllConfigurationItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "configurationitem")]
            HttpRequest request, ILogger log)
        {
            log.LogInformation("GetConfigurationItem Function");

            var collection = new DatabaseService<ConfigurationItem>().GetCollection();

            return new OkObjectResult(collection);
        }

        [FunctionName("CreateConfigurationItem")]
        public async Task<IActionResult> CreateConfigurationItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "configurationitem")]
            HttpRequest request, ILogger log)
        {
            log.LogInformation("CreateConfigurationItem Function");

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ConfigurationItem>(requestBody);

            data.Created = DateTimeOffset.Now;
            data.Updated = DateTimeOffset.Now;

            // Validate + Add user to configurationItem

            var databaseAccess = new DatabaseService<ConfigurationItem>();
            await databaseAccess.SaveItem(data);

            var responseMessage = "Succesfully created configuration item";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("DeleteConfigurationItemById")]
        public async Task<IActionResult> DeleteConfigurationItemById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "configurationitem/{id}")]
            HttpRequest request, ILogger log, string id)
        {
            log.LogInformation("DeleteConfigurationItemById Triggered");

            var databaseAccess = new DatabaseService<ConfigurationItem>();
            await databaseAccess.DeleteItem(id);

            var responseMessage = "Succesfully deleted configuration item";

            return new OkObjectResult(responseMessage);
        }
    }
}
