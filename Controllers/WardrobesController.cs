using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using WardRobe.Data;
using WardRobe.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WardRobe.Views.Wardrobes
{
    public class WardrobesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WardrobesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Wardrobes
        public async Task<IActionResult> Index(string searchString)
        {
       
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);

            var userid = _userManager.GetUserId(HttpContext.User);

            var wardrobe = from m in _context.Wardrobe select m;
            IQueryable<string> TypeQuery = from m in _context.Wardrobe
                                           orderby m.Name
                                           select m.Name;
            IEnumerable<SelectListItem> items =
                new SelectList(await TypeQuery.Distinct().ToListAsync());

            ViewBag.WardrobeName = items;

            if (!String.IsNullOrEmpty(searchString))
            {
                wardrobe = wardrobe.Where(m => m.Name == searchString && m.UserId.Contains(userid));
            }

            if (!String.IsNullOrEmpty(userid))
            {
                wardrobe = wardrobe.Where(m => m.UserId.Contains(userid));
            }

            return View(await wardrobe.AsNoTracking().ToListAsync());
        }

        // GET: Wardrobes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardrobe = await _context.Wardrobe
                .FirstOrDefaultAsync(m => m.ID == id);
            if (wardrobe == null)
            {
                return NotFound();
            }

            return View(wardrobe);
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            //Link to the appsettings.json file
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            //Once link, time to read content from connection string
            CloudStorageAccount objectaccount =
                CloudStorageAccount.Parse(configure["ConnectionStrings:wardrobe3"]);
            CloudBlobClient blobclient = objectaccount.CreateCloudBlobClient();

            //create the container inside the stroage account
            CloudBlobContainer container = blobclient.GetContainerReference("wardrobe");
            return container;
        }

        public void GetBlobName()
        {
            CloudBlobContainer container = GetCloudBlobContainer();

        }

        // GET: Wardrobes/Create
        public IActionResult Create()
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);
            return View();
        }

        // POST: Wardrobes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Category,Name,Brand,PurchaseDate,Price,WornTimes,ImageUrl,FileName,UserId")] Wardrobe wardrobe, List<IFormFile> files)
        {
            ViewBag.userid = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid)
            {
                //get temporary filepath 
                var filepath = Path.GetTempFileName();

                foreach (var FormFile in files)
                {
                    //chack the file 
                    if (FormFile.Length <= 0)
                    {
                        TempData["message"] = "Please upload an image file";
                    }

                    //If file is valid proceed to transfer data 
                    {
                        //Get the information of the container
                        CloudBlobContainer container = GetCloudBlobContainer();
                        //create the container if not exist in the storage
                        ViewBag.Success = container.CreateIfNotExistsAsync().Result;
                        ViewBag.BlobContainerName = container.Name; //get the container name

                        //Give a name for the blob 
                        CloudBlockBlob blob = container.GetBlockBlobReference(Path.GetFileName(FormFile.FileName));
                        try
                        {
                            using (var stream = FormFile.OpenReadStream())
                            {
                                await blob.UploadFromStreamAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["message"] = ex.ToString();
                        }

                        //get uri of the uploaded blob and save in database
                        var blobUrl = blob.Uri.AbsoluteUri;
                        wardrobe.ImageUrl = blobUrl.ToString();
                        wardrobe.FileName = FormFile.FileName.ToString();
                        _context.Add(wardrobe);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
                TempData["createmessage"] = "Please upload an image for your clothes";
                return RedirectToAction(nameof(Create));

            }
            return View(wardrobe);
        }

        // GET: Wardrobes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardrobe = await _context.Wardrobe.FindAsync(id);
            if (wardrobe == null)
            {
                return NotFound();
            }
            return View(wardrobe);
        }

        // POST: Wardrobes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Category,Name,Brand,PurchaseDate,Price,WornTimes,ImageUrl,FileName,UserId")] Wardrobe wardrobe, List<IFormFile> files, string filename)
        {
            if (id != wardrobe.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var filepath = Path.GetTempFileName();

                foreach (var FormFile in files)
                {
                    if (FormFile.Length <= 0)
                    {
                        TempData["editmessage"] = "Please upload a proper image file";
                    }

                    else
                    {
                        if (filename != null)
                        {
                            //delete file from blob
                            CloudBlobContainer container1 = GetCloudBlobContainer();
                            CloudBlockBlob blobfile = container1.GetBlockBlobReference(filename);
                            string name = blobfile.Name;
                            var result = blobfile.DeleteIfExistsAsync().Result;

                            if (result == false)
                            {
                                TempData["message"] = "Unable to delete file";
                            }
                            else
                            {
                                TempData["message"] = "File is deleted";
                            }
                        }
                        //first all, get the container information
                        CloudBlobContainer container = GetCloudBlobContainer();
                        //give a name for the blob
                        CloudBlockBlob blob = container.GetBlockBlobReference(Path.GetFileName(FormFile.FileName));
                        try
                        {
                            using (var stream = FormFile.OpenReadStream())
                            {
                                await blob.UploadFromStreamAsync(stream);
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["message"] = ex.ToString();
                        }

                        // get the uri of the specific uploaded blob and save it
                        var blobUrl = blob.Uri.AbsoluteUri;
                        wardrobe.ImageUrl = blobUrl.ToString();
                        wardrobe.FileName = FormFile.FileName.ToString();
                        _context.Update(wardrobe);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
                try
                {
                    _context.Update(wardrobe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardrobeExists(wardrobe.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(wardrobe);
        }

        // GET: Wardrobes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wardrobe = await _context.Wardrobe
                .FirstOrDefaultAsync(m => m.ID == id);
            if (wardrobe == null)
            {
                return NotFound();
            }

            return View(wardrobe);
        }

        // POST: Wardrobes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string filename)
        {
            var wardrobe = await _context.Wardrobe.FindAsync(id);

            if (filename != null)
            {
                //delete file from blob
                CloudBlobContainer container = GetCloudBlobContainer();
                CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                string name = blob.Name;
                var result = blob.DeleteIfExistsAsync().Result;

                if (result == false)
                {
                    TempData["message"] = "Unable to delete image";
                }
                else
                {
                    TempData["message"] = "Image is deleted";
                }
            }

            _context.Wardrobe.Remove(wardrobe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardrobeExists(int id)
        {
            return _context.Wardrobe.Any(e => e.ID == id);
        }
    }
}
