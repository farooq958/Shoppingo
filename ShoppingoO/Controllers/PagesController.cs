using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingoO.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingoO.Controllers
{
    public class PagesController : Controller
    {
        private readonly Infrastructure.ShoppingoOcontext context;
        public PagesController(Infrastructure.ShoppingoOcontext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Page(string slug)
        {
            if (slug == null)
            {
                return RedirectToAction("Index","Products");
//return View(await context.Pages.Where(x =>x.Slug=="home" ).FirstOrDefaultAsync());   
            }
            Models.Page pag = await context.Pages.Where(x => x.Slug==slug).FirstOrDefaultAsync();
            if (pag == null)
            {
                return NotFound();
            }    

            return View(pag);
        }
    }
}
