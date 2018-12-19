using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    // If you do not add this [Area("Admin")] it will not be able to identify this controller
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        // When dealing with CRUD operatons you always need an ApplicationDbContext object or the database object
        // USE DEPENDENCY INJECTION
        private readonly ApplicationDbContext _db;

        // You need a constructor for dependency injection so that it always grabs the db when the class is utilized
        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            // Gets the table from the database and passes it to the view in a list
            return View(_db.ProductTypes.ToList());
        }

        // GET create action method
        public IActionResult Create()
        {
            return View();
        }

        /* ValidateAntiForgeryToken is a token that is sent with the HttpPost request to the server where the server
         checks if the token is valid or not. If valid it knows that the request has not been changed by any hackers in 
         any way. Comes with ASP.NET*/
         // POST create action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if(ModelState.IsValid)
            {
                _db.Add(productTypes);
                await _db.SaveChangesAsync();
                /* You redirect to the index action method because you then you want to display the index page with all of
                 the producttypes. If you use the nameof keyword it will help you to not have a spelling mistake because it will
                 throw an error letting you know there is no implementation of that word in the context*/
                return RedirectToAction(nameof(Index));
            }
            // If there is an error we will return to the view and you pass it the product types so it can display the error message
            return View(productTypes);
        }
    }
}