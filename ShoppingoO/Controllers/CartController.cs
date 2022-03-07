using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ShoppingoO.Infrastructure;
using ShoppingoO.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingoO.Controllers
{
    public class CartController : Controller
    {
        private readonly Infrastructure.ShoppingoOcontext context ;
        public CartController(Infrastructure.ShoppingoOcontext context)
        {
this.context = context;

        }
        //Get  /cart
        public IActionResult Index()
        {
            
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartViewModel cartVM = new CartViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Price * x.Quantity)
            };
           // cartViewModel.CartItems=cart;
           // cartViewModel.GrandTotal = cart.Sum(x =>x.Price*x.Quantity);
            
            return View(cartVM);
        }
        //Get cart/add/id
        public async Task<IActionResult> Add(int id)
        {
            
            var product = await context.Products.FindAsync(id);
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem Cart = cart.Where(x => x.ProductId==id).FirstOrDefault();
            if (Cart == null)
            {
                cart.Add(new CartItem (product));  
            }
            else
            {
                Cart.Quantity+=1;

            }
            HttpContext.Session.SetJson("Cart", cart);
            //RouteContext routeContext;
             if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            return RedirectToAction("Index");
            else
               return ViewComponent("SmallCart");
        }


        public  IActionResult Decrease(int id)
        {

           
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
            CartItem Cart = cart.Where(x => x.ProductId==id).FirstOrDefault();
            if (Cart.Quantity > 1)
            {
                --Cart.Quantity;
            }
            else
            {
                cart.RemoveAll(c => c.ProductId==id);
            }
           
            if (cart.Count==0)
            {
                HttpContext.Session.Remove("Cart");

            }
            else
            {
 HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }


        public  IActionResult Remove(int id)
        {


            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
           
                cart.RemoveAll(c => c.ProductId==id);
            

            if (cart.Count==0)
            {
                HttpContext.Session.Remove("Cart");

            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Clear(int id)
        {


            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

          


                HttpContext.Session.Remove("Cart");


            //return RedirectToAction("Index");
            //return Redirect("/");
            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return Redirect(Request.Headers["Referer"].ToString());

            return Ok();
        }

    }
}
