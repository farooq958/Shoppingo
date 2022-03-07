using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ShoppingoO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingoO.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        // GET /admin/roles
        public IActionResult Index() => View(roleManager.Roles);

        // GET /admin/roles/create
        public IActionResult Create() => View();
        
        // POST /admin/roles/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([MinLength(2), Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    TempData["Success"] = "The role has been created!";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors) ModelState.AddModelError("", error.Description);
                }
            }

            ModelState.AddModelError("", "Minimum length is 2");
            return View();
        }
        
        // GET /admin/roles/edit/5
       public async Task<IActionResult> Edit(string id)
        {
            IdentityRole identityRole=await roleManager.FindByIdAsync(id);
            var members=new List<AppUser>();
            var nonmembers = new List<AppUser>();
            foreach (AppUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, identityRole.Name) ? members : nonmembers;
            
            list.Add(user);
            }

            return View(
                new RoleEdit { Role=identityRole,Members=members,NonMembers=nonmembers}
                ); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleEdit roleEdit)
        {
            IdentityResult result;

            foreach (string userId in roleEdit.AddIds ?? new string[] { })
            {
                AppUser user = await userManager.FindByIdAsync(userId);
                result = await userManager.AddToRoleAsync(user, roleEdit.RoleName);
            }

            foreach (string userId in roleEdit.DeleteIds ?? new string[] { })
            {
                AppUser user = await userManager.FindByIdAsync(userId);
                result = await userManager.RemoveFromRoleAsync(user, roleEdit.RoleName);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Delete(string id)
        {
            IdentityResult result;
            IdentityRole role = await roleManager.FindByIdAsync(id);
          
            
                result = await roleManager.DeleteAsync(role);


                if (result.Succeeded)
                {
                    TempData["Success"]="Role has been Deleted";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    return View("Index");
                }
            
        }
    }
}