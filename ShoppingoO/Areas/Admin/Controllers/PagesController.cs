using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingoO.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingoO.Models;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingoO.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly Infrastructure.ShoppingoOcontext context;
        public PagesController(Infrastructure.ShoppingoOcontext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
           var pages =  context.Pages;
            var p=await pages.OrderBy(x => x.Sorting).ToListAsync();    
         //   List<Page> pagesList = await p.ToListAsync();

            return View(p);
        }
        //Get details of product
        public async Task<IActionResult> Details(int id)
        {
            var pages = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);
            // var p = await pages.OrderBy(x => x.Sorting).ToListAsync();
            //   List<Page> pagesList = await p.ToListAsync();
            if (pages == null)
            {
                return NotFound();
            }
            return View(pages);
        }
        public  IActionResult Create()
        {
          
            
            return View();
        }
        [HttpPost] //ADMIN/PAGES/Create
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists.");
                    return View(page);
                }

                context.Add(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";

                return RedirectToAction("Index");
            }

            return View(page);
        }
//get admin/pages/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var pages = await context.Pages.FindAsync(id);
            // var p = await pages.OrderBy(x => x.Sorting).ToListAsync();
            //   List<Page> pagesList = await p.ToListAsync();
            if (pages == null)
            {
                return NotFound();
            }
            return View(pages);
        }
        [HttpPost] //ADMIN/PAGES/Edit
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Models.Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Id ==1 ? "home" :page.Title.ToLower().Replace(" ", "-");
                
                

                var slug = await context.Pages.Where(x => x.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists.");
                    return View(page);
                }

                context.Update(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been Edited!";

                return RedirectToAction("Edit",new {Id = page.Id });
            }

            return View(page);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var pages = await context.Pages.FindAsync(id);
            // var p = await pages.OrderBy(x => x.Sorting).ToListAsync();
            //   List<Page> pagesList = await p.ToListAsync();
            if (pages == null)
            {
                TempData["Error"]="The Page DoesNot Exist";
            }
            else
            {
                context.Remove(pages);
                await context.SaveChangesAsync();
                TempData["Success"]="The Page has been Deleted Successfully!";
            }
            return RedirectToAction("Index");
        }



        [HttpPost] //ADMIN/PAGES/Reorder
        
        public async Task<IActionResult> reorder(int [] id)
        {
            int count = 1;
            foreach (var pageid in id)
            {
                Models.Page page = await context.Pages.FindAsync(pageid);
                page.Sorting=count;
                context.Update(page);
                await context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }
    }
}
