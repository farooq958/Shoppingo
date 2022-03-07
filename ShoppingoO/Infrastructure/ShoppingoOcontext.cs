using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingoO.Models;
using System.Diagnostics.CodeAnalysis;

namespace ShoppingoO.Infrastructure
{
    public class ShoppingoOcontext : IdentityDbContext<AppUser>
    {
        public ShoppingoOcontext( DbContextOptions<ShoppingoOcontext> options) : base(options)
        {
       } 
        
        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
