using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingoO.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingoO.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly Infrastructure.ShoppingoOcontext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductsController(Infrastructure.ShoppingoOcontext context,IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment =   webHostEnvironment;     
        }
        public async Task<IActionResult> Index(int p=1)
        {
            int pageSize = 5;
            var products = context.Products.OrderByDescending(x => x.Id)
                                            .Include(x => x.Category)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }
        public IActionResult Create()
        {
            ViewBag.CategoryId= new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name");

            return View();
        }

    
    [HttpPost] //ADMIN/Product/Create
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Models.Product product)
    {
            ViewBag.CategoryId= new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            if (ModelState.IsValid)
        {
            product.Slug = product.Name.ToLower().Replace(" ", "-");
           

            var slug = await context.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug);
            if (slug != null)
            {
                ModelState.AddModelError("", "The Product already exists.");
                return View(product);
            }
                string imagename = "noimage.png";
                if(product.ImageUpload!= null)
                {
                    string uploaddir = Path.Combine(webHostEnvironment.WebRootPath, "lib/Media/Productimage");
                    imagename = Guid.NewGuid().ToString()+ "_" + product.ImageUpload.FileName;
                    string filepath = Path.Combine(uploaddir, imagename);
                FileStream  fs = new FileStream(filepath, FileMode.Create);
                   await product.ImageUpload.CopyToAsync(fs);
                    fs.Close(); 
                        }
                product.Image= imagename;
            context.Add(product);
            await context.SaveChangesAsync();

            TempData["Success"] = "The Product has been added!";

            return RedirectToAction("Index");
        }

        return View(product);
    }


      //  Admin/product/details
        public async Task<IActionResult> Details(int id)
        {
            var Product = await context.Products.Include(x=>x.Category).FirstOrDefaultAsync(x=>x.Id == id);
            // var p = await pages.OrderBy(x => x.Sorting).ToListAsync();
            //   List<Page> pagesList = await p.ToListAsync();
            if (Product == null)
            {
                return NotFound();
            }
            return View(Product);
        }


        //get admin/Product/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var Product = await context.Products.FindAsync(id);
            // var p = await pages.OrderBy(x => x.Sorting).ToListAsync();
            //   List<Page> pagesList = await p.ToListAsync();
            if (Product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId= new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name",Product.CategoryId);
          
            
            return View(Product);
        }

        [HttpPost] //ADMIN/Product/Edit
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Models.Product product)
        {
            ViewBag.CategoryId= new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");


                var slug = await context.Products.Where(x=>x.Id!= id).FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Product already exists.");
                    return View(product);
                }
               
                if (product.ImageUpload!= null)
                {
                    string uploaddir = Path.Combine(webHostEnvironment.WebRootPath, "lib/Media/Productimage");
                    if (!String.Equals(product.Image,"noimage.png"))
                    {
                        string oldimagepath = Path.Combine(uploaddir, product.Image);
                        if (System.IO.File.Exists(oldimagepath))
                        {
                            System.IO.File.Delete(oldimagepath);
                        }
                    }
                    
                  var  imagename = Guid.NewGuid().ToString()+ "_" + product.ImageUpload.FileName;
                    string filepath = Path.Combine(uploaddir, imagename);
                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close(); 
                    product.Image= imagename;
                }
               
                context.Update(product);
                await context.SaveChangesAsync();

                TempData["Success"] = "The Product has been Edited!";

                return RedirectToAction("Index");
            }

            return View(product);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Product = await context.Products.FindAsync(id);
            // var p = await pages.OrderBy(x => x.Sorting).ToListAsync();
            //   List<Page> pagesList = await p.ToListAsync();
            if (Product == null)
            {
                TempData["Error"]="The Product DoesNot Exist";
            }
            else
            {
                string uploaddir = Path.Combine(webHostEnvironment.WebRootPath, "lib/Media/Productimage");
                if (!String.Equals(Product.Image, "noimage.png"))
                {
                    string oldimagepath = Path.Combine(uploaddir, Product.Image);
                    if (System.IO.File.Exists(oldimagepath))
                    {
                        System.IO.File.Delete(oldimagepath);
                    }
                }
                context.Remove(Product);
                await context.SaveChangesAsync();
                TempData["Success"]="The Product has been Deleted Successfully!";
            }
            return RedirectToAction("Index");
        }
    }
}
