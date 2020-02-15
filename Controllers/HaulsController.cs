using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using WardRobe.Models;
using Microsoft.AspNetCore.Identity;

namespace WardRobe.Controllers
{
    public class HaulsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public HaulsController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public CloudStorageAccount connectionstrg()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();
            CloudStorageAccount storageaccount =
                CloudStorageAccount.Parse(configuration["ConnectionStrings:wardrobe2"]);
            return storageaccount;
        }

        public IActionResult Index(string Category)
        {
            if (!String.IsNullOrEmpty(Category))
            {
                var userid = _userManager.GetUserId(HttpContext.User);

                CloudStorageAccount storage = connectionstrg();
                CloudTableClient tableClient = storage.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("HaulTable");

                TableQuery<Haul> query = new TableQuery<Haul>();
                List<Haul> hauls = new List<Haul>();
                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<Haul> resultSegment = table.ExecuteQuerySegmentedAsync(query, token).Result;
                    token = resultSegment.ContinuationToken;

                    foreach (Haul haul in resultSegment.Results)
                    {
                        hauls.Add(haul);
                    }
                }
                while (token != null);

                var data = from m in hauls select m;

                if (!String.IsNullOrEmpty(userid))
                {
                    data = data.Where(m => m.UserId.Contains(userid) && m.PartitionKey.Contains(Category));
                }

                return View(data);

            }
            else
            {
                var userid = _userManager.GetUserId(HttpContext.User);

                CloudStorageAccount storage = connectionstrg();
                CloudTableClient tableclient = storage.CreateCloudTableClient();
                CloudTable table = tableclient.GetTableReference("HaulTable");
                ViewBag.result = table.CreateIfNotExistsAsync().Result;
                ViewBag.TableName = table.Name;

                TableQuery<Haul> query = new TableQuery<Haul>();
                List<Haul> hauls = new List<Haul>();
                TableContinuationToken token = null;
                do
                {
                    TableQuerySegment<Haul> resultSegment = table.ExecuteQuerySegmentedAsync(query, token).Result;
                    token = resultSegment.ContinuationToken;

                    foreach (Haul haul in resultSegment.Results)
                    {
                        hauls.Add(haul);
                    }
                }
                while (token != null);

                var data = from m in hauls select m;

                if (!String.IsNullOrEmpty(userid))
                {
                    data = data.Where(m => m.UserId.Contains(userid));
                }

                return View(data);
            }
        }

        public IActionResult Create()
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("category, clothes, brand, description, UserId")] Haul haul)
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid)
            {
                CloudStorageAccount storage = connectionstrg();
                CloudTableClient tableClient = storage.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("HaulTable");
                string rowkey = Guid.NewGuid().ToString("N");
                Haul hauls = new Haul(haul.category, rowkey);
                hauls.clothes = haul.clothes;
                hauls.brand = haul.brand;
                hauls.description = haul.description;
                hauls.UserId = haul.UserId;

                // Create the TableOperation that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(hauls);

                // Execute the insert operation.
                await table.ExecuteAsync(insertOperation);

                return RedirectToAction(nameof(Index));
            }
            return View(haul);
        }
        public async Task<IActionResult> Edit(string partitionkey, string rowkey)
        {
            if (rowkey == null)
            {
                return NotFound();
            }
            if (partitionkey == null)
            {
                return NotFound();
            }

            CloudStorageAccount storage = connectionstrg();
            CloudTableClient tableclient = storage.CreateCloudTableClient();
            CloudTable table = tableclient.GetTableReference("HaulTable");

            TableOperation retrieveOperation = TableOperation.Retrieve<Haul>(partitionkey, rowkey);

            TableResult result = await table.ExecuteAsync(retrieveOperation);

            Haul hauls = new Haul();

            if (result.Result != null)
            {
                hauls = ((Haul)result.Result);
            }

            return View(hauls);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("PartitionKey, RowKey, category, clothes, brand, description, UserId")] Haul haul)
        {
            if (ModelState.IsValid)
            {
                CloudStorageAccount storage = connectionstrg();
                CloudTableClient tableclient = storage.CreateCloudTableClient();
                CloudTable table = tableclient.GetTableReference("HaulTable");

                Haul hauls = new Haul(haul.PartitionKey, haul.RowKey);
                hauls.clothes = haul.clothes;
                hauls.brand = haul.brand;
                hauls.description = haul.description;
                hauls.UserId = haul.UserId;
                hauls.ETag = "*";

                // Update the details into table storage.
                TableOperation UpdateOperation = TableOperation.Replace(hauls);

                // Execute the update operation.
                await table.ExecuteAsync(UpdateOperation);

                return RedirectToAction(nameof(Index));
            }
            return View(haul);
        }

        public async Task<IActionResult> Delete(string partitionkey, string rowkey)
        {
            if (rowkey == null)
            {
                return NotFound();
            }
            if (partitionkey == null)
            {
                return NotFound();
            }

            CloudStorageAccount storage = connectionstrg();
            CloudTableClient tableclient = storage.CreateCloudTableClient();
            CloudTable table = tableclient.GetTableReference("HaulTable");

            TableOperation retrieveOperation = TableOperation.Retrieve<Haul>(partitionkey, rowkey);

            TableResult result = await table.ExecuteAsync(retrieveOperation);

            Haul hauls = new Haul();

            if (result.Result != null)
            {
                hauls = ((Haul)result.Result);
            }

            return View(hauls);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string partitionkey, string rowkey)
        {
            CloudStorageAccount storageaccount = connectionstrg();
            CloudTableClient tableClient = storageaccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("SymptomTable");

            TableOperation deleteOperation = TableOperation.Delete(new Haul(partitionkey, rowkey) { ETag = "*" });
            TableResult result = table.ExecuteAsync(deleteOperation).Result;

            return RedirectToAction(nameof(Index));
        }
    }
}