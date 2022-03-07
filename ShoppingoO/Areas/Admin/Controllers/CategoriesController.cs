using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingoO.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingoO.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly Infrastructure.ShoppingoOcontext context;
        public CategoriesController(Infrastructure.ShoppingoOcontext context)
        {
         this.context = context;
        }
        public async Task<IActionResult> Index()
        {

            return View(await context.Categories.OrderBy(x => x.Sorting).ToListAsync());
        }
        //get/categories/create
        public IActionResult Create()
        {


            return View();
        }
        [HttpPost] //ADMIN/Category/Create
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                category.Sorting = 100;

                var slug = await context.Categories.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists.");
                    return View(category);
                }

                context.Add(category);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";

                return RedirectToAction("Index");
            }

            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await context.Categories.FindAsync(id);
            // var p = await pages.OrderBy(x => x.Sorting).ToListAsync();
            //   List<Page> pagesList = await p.ToListAsync();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost] //ADMIN/PAGES/Edit
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id,Models.Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");



                var slug = await context.Pages.Where(x => x.Id != Id).FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Category already exists.");
                    return View(category);
                }

                context.Update(category);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been Edited!";

                return RedirectToAction("Edit", new {  Id });
            }

            return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var pages = await context.Categories.FindAsync(id);
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
        [HttpPost] //ADMIN/category/Reorder

        public async Task<IActionResult> reorder(int[] id)
        {
            int count = 1;
            foreach (var pageid in id)
            {
                Models.Category category = await context.Categories.FindAsync(pageid);
                category.Sorting=count;
                context.Update(category);
                await context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }


    }
}
