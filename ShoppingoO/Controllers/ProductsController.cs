using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingoO.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingoO.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly Infrastructure.ShoppingoOcontext context;
       
        public ProductsController(Infrastructure.ShoppingoOcontext context)
        {
            this.context = context;
            
        }
        //Get /products/
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 5;
            var products = context.Products.OrderByDescending(x => x.Id)
                                            
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }

        //Get /products/category
        public async Task<IActionResult> ProductsbyCategory( string CategorySlug ,int p = 1)
        {

            var category =await context.Categories.Where(x=>x.Slug ==CategorySlug).FirstOrDefaultAsync();
            if (category == null)
return RedirectToAction("Index"); 
            int pageSize = 5;
            var products = context.Products.OrderByDescending(x => x.Id)
                .Where(x => x.CategoryId == category.Id) 
                .Skip((p - 1) * pageSize)
                 .Take(pageSize);                           

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Where(x => x.CategoryId == category.Id).Count() / pageSize);
            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug= CategorySlug; 
            return View(await products.ToListAsync());
        }
    }
}
