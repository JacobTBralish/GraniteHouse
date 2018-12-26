using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        // Bind property makes it so that if you are ever posting anything or retrieving anything it will automatically bind this
        // ProductViewModel. So if you are on a create page and the user posts all the details it will be binded to that PoductsViewModel
        // and you do not have to insert it into the arguments of the action method.
        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }

        // GET : Products create
        public IActionResult Create()
        {
            return View(ProductsVM);
        }

        //POST : Products create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if(!ModelState.IsValid)
            {
                return View(ProductsVM);
            }
            _db.Products.Add(ProductsVM.Products);
            await _db.SaveChangesAsync();

            // Product Image being saved IF THE FILE WAS UPLOADED
            // webRootPath finds the path of the application
            string webRootPath = _hostingEnvironment.WebRootPath;
            // files has any of the files that were uploaded from the view 
            var files = HttpContext.Request.Form.Files;


            // To make this possible we needed to retrieve the product from the object above so we could use its Id, if we did this
            // before hand, we would not have had the object and it would have returned null
            var productsFromDb = _db.Products.Find(ProductsVM.Products.Id);

            // Checks if the user has uploaded any files
            if(files.Count != 0)
            {
                // This means that image has been uploaded and finds the path
                // SD is being imported from the utilities folder which holds the method for the path to the images folder in wwwroot
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                // This gives the extension of the file
                var extension = Path.GetExtension(files[0].FileName);
                // Use the filestream object to copy the file from the uploaded to the server
                // FileStream requires the destination path
                // The second parameter in Combine renames the file to the productID with the extension
                // The filemode.create creates the file on the server
                using (var fileStream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension), FileMode.Create))
                {
                    //This will move the file to the server and also rename it
                    files[0].CopyTo(fileStream);
                }

                // That way in the products.image we will have the exact path of where the image is saved on the server with the
                // file name and extension. This is all for the condition if the file was uploaded
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + extension;

            }
            else
            {
                // IF THE FILE WAS NOT UPLOADED
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                // Now we will make it so that the image is copied from the server and renamed to the productId
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id+".png");

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsVM.Products.Id + ".png";
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}