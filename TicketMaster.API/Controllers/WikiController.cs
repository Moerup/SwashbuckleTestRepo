using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;
using TicketMaster.API.Services;
using TicketMaster.API.Models;

namespace TicketMaster.API.Controllers
{
    public class WikiController
    {
        /// <summary>
        /// Get a specific wiki item by ID
        /// </summary>
        /// <param name="Id">Id of wiki item</param>
        /// <returns></returns>
        [FunctionName("GetWikiItem")]
        public static async Task<IActionResult> GetWikiItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wiki/{Id}")]
            HttpRequest request,
            ILogger log, string Id)
        {
            log.LogInformation("GetWiki Function");

            var collection = new DatabaseService<WikiItem>().GetCollection();
            var requestedItem = collection.Where(x => x.Id == Id);

            return new OkObjectResult(requestedItem);
        }

        /// <summary>
        /// Get a dump of all wiki items.
        /// </summary>
        /// <returns></returns>
        [FunctionName("GetAllWikiItems")]
        public static async Task<IActionResult> GetAllWikiItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wiki/all")]
             HttpRequest request,
            ILogger log)
        {
            log.LogInformation("GetAllWikiItems Function");

            var collection = new DatabaseService<WikiItem>().GetCollection();

            return new OkObjectResult(collection);
        }

        /// <summary>
        /// Get wiki items paginated, with skip and take parameters. Ordered by created date descending.
        /// </summary>
        /// <param name="skip">Amount to skip</param>
        /// <param name="take">Amount to take</param>
        /// <returns></returns>
        [FunctionName("GetWikiItemsPaginated")]
        public static async Task<IActionResult> GetWikiItemsPaginated(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wiki/{skip}/{take}")]
             HttpRequest request,
            ILogger log, int skip, int take)
        {
            log.LogInformation("GetWikiItems paginated Function");

            var collection = new DatabaseService<WikiItem>().GetCollection();
            var requestedItems = collection.OrderByDescending(x => x.Created).Skip(skip).Take(take);

            return new OkObjectResult(requestedItems);
        }

        /// <summary>
        /// Get wiki items by category id paginated with skip and take parameters.
        /// </summary>
        /// <param name="Id">Category Id</param>
        /// <param name="skip">Amount to skip</param>
        /// <param name="take">Amount to take</param>
        /// <returns></returns>
        [FunctionName("GetWikiItemsByCategoryPaginated")]
        public static async Task<IActionResult> GetWikiItemsByCategoryPaginated(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wiki/WikiByCategory/{Id}/{skip}/{take}")]
            HttpRequest request,
            ILogger log, string Id, int skip, int take)
        {
            log.LogInformation("GetWikiItems paginated Function");

            var category = new DatabaseService<WikiCategory>().GetCollection().Where(x => x.Id == Id);
            var collection = new DatabaseService<WikiItem>().GetCollection();
            var sortedCollection = collection.Where(x => x.WikiCategoryIds.Contains(Id));
            var requestedItems = sortedCollection.OrderByDescending(x => x.Created).Skip(skip).Take(take);

            return new OkObjectResult(requestedItems);
        }

        /// <summary>
        /// Get all wiki items by category
        /// </summary>
        /// <param name="Id">Category Id</param>
        /// <returns></returns>
        [FunctionName("GetWikiItemsByCategory")]
        public static async Task<IActionResult> GetWikiItemsByCategory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "wiki/WikiByCategory/{Id}")]
            HttpRequest request,
            ILogger log, string Id)
        {
            log.LogInformation("GetWikiItems by category Function");
            var category = new DatabaseService<WikiCategory>().GetCollection().Where(x => x.Id == Id);
            var collection = new DatabaseService<WikiItem>().GetCollection();
            var sortedCollection = collection.Where(x => x.WikiCategoryIds.Contains(Id));
            var requestedItems = sortedCollection.OrderByDescending(x => x.Created);

            return new OkObjectResult(requestedItems);
        }

        /// <summary>
        /// Create wiki item
        /// </summary>
        /// <param name="request"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateWikiItem")]
        public static async Task<IActionResult> CreateWikiItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "wiki")]
             HttpRequest request,
            ILogger log)
        {
            log.LogInformation("CreateWikiItem Function");

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<WikiItem>(requestBody);
            data.Created = DateTime.Now;
            data.Updated = DateTime.Now;
            data.AuthorId = string.Empty; // need proper user id.
            data.UpdatedByUsers = new List<string>() { string.Empty };// need proper user id.

            var dbAccess = new DatabaseService<WikiItem>();
            await dbAccess.SaveItem(data);

            var responseMessage = $"Successfully added new wiki by provided data.";

            return new OkObjectResult(responseMessage);
        }

        /// <summary>
        /// Update a single wiki Item.
        /// </summary>
        /// <param name="Id">Id of Wiki Item.</param>
        /// <param name="request">Model to be updated.</param>
        /// <returns></returns>
        [FunctionName("UpdateWikiItem")]
        public static async Task<IActionResult> UpdateWikiItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "wiki/{Id}")]
             HttpRequest request,
            ILogger log, string Id)
        {
            log.LogInformation("UpdateWikiItem Function");

            var wikiItem = new DatabaseService<WikiItem>().GetCollection().Where(x => x.Id == Id).FirstOrDefault();
            if (wikiItem == null)
            {
                return new NotFoundObjectResult($"Could not find the category with id {Id}");
            }

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<WikiItem>(requestBody);
            data.Id = wikiItem.Id; //assign id, since it will be missing.
            data.Created = wikiItem.Created; // re-assign since it should keep the original one.
            data.Updated = DateTime.Now;
            data.AuthorId = wikiItem.AuthorId; // re-assign since it should keep the original one.

            List<string> updateUserIdsList = wikiItem.UpdatedByUsers;
            updateUserIdsList.Add(string.Empty); // need proper user id.

            data.UpdatedByUsers = updateUserIdsList;

            var dbAccess = new DatabaseService<WikiItem>();
            await dbAccess.UpdateItem(data); // it actually replaces it...

            var responseMessage = $"Successfully updated wiki item by provided data.";

            return new OkObjectResult(responseMessage);
        }
    }
}
