using ShoppingoO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingoO.Infrastructure;

namespace CmsShoppingCart.Infrastructure
{
    public class MainMenuViewComponent : ViewComponent
    {
        private readonly ShoppingoO.Infrastructure.ShoppingoOcontext context;

        public MainMenuViewComponent(ShoppingoO.Infrastructure.ShoppingoOcontext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pages = await GetPagesAsync();
            return View(pages);
        }

        private Task<List<Page>> GetPagesAsync()
        {
            return context.Pages.OrderBy(x => x.Sorting).ToListAsync();
        }
    }
}
