using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SWZSR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Data
{
    public static class IdentitySeedData
    {
        public static async Task EnsurePopulatedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Client").Result)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole("Client"));
            }
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!roleManager.RoleExistsAsync("Mechanic").Result)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole("Mechanic"));
            }

            if (userManager.FindByEmailAsync("admin@example.com").Result == null)
            {
                var user = new ApplicationUser
                {
                    Email = "admin@example.com",
                    UserName = "admin@example.com",
                    PhoneNumber = "000000000",
                    Firstname = "Admin",
                    Lastname = "Admin",    
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(user, "P@ssw0rd");
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

            if (userManager.FindByEmailAsync("client@example.com").Result == null)
            {
                var user = new ApplicationUser
                {
                    Email = "client@example.com",
                    UserName = "client@example.com",
                    PhoneNumber = "000000001",
                    Firstname = "Client",
                    Lastname = "Client",
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(user, "P@ssw0rd");
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Client").Wait();
                }
            }

            if (userManager.FindByEmailAsync("mechanic@example.com").Result == null)
            {
                var user = new ApplicationUser
                {
                    Email = "mechanic@example.com",
                    UserName = "mechanic@example.com",
                    PhoneNumber = "000000002",
                    Firstname = "Mechanic",
                    Lastname = "Mechanic",
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(user, "P@ssw0rd");
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Mechanic").Wait();
                }
            }
        }

        //public static void EnsurePopulated(IApplicationBuilder app)
        //{
        //    ApplicationDbContext db = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
        //    db.Database.Migrate();



        //    if (!db.Roles.Any())
        //    {
        //        db.Roles.AddRange(
        //            new Microsoft.AspNetCore.Identity.IdentityRole { }
        //        );
        //        db.SaveChanges();
        //    }

        //    if (!db.Users.Any())
        //    {
        //        db.Services.AddRange(
        //            new Service { Name = "Inny" },
        //            new Service { Name = "Przegląd duży", Price = 199.00M, EstimatedTime = 4.0 },
        //            new Service { Name = "Przegląd mały", Price = 99.00M, EstimatedTime = 3.0 },
        //            new Service { Name = "Przegląd duży (24h)", Price = 239.00M, EstimatedTime = 1.0 },
        //            new Service { Name = "Przegląd mały (24h)", Price = 129.00M, EstimatedTime = 1.0 },
        //            new Service { Name = "Przegląd gwarancyjny", Price = 40.00M, EstimatedTime = 2.0 },
        //            new Service { Name = "Centrowanie koła", Price = 35.00M, EstimatedTime = 2.0 },
        //            new Service { Name = "Wymiana szprychy + centrowanie", Price = 40.00M, EstimatedTime = 2.0 },
        //            new Service { Name = "Wymiana opony", Price = 15.00M, EstimatedTime = 1.0 },
        //            new Service { Name = "Wymiana dętki", Price = 15.00M, EstimatedTime = 1.0 },
        //            new Service { Name = "Wymiana dętki + dętka", Price = 36.00M, EstimatedTime = 1.0 },
        //            new Service { Name = "Smarowanie łańcucha", Price = 0.00M, EstimatedTime = 1.0 },
        //            new Service { Name = "Mycie i smarowanie łańcucha", Price = 30.00M, EstimatedTime = 1.0 },
        //            new Service { Name = "Regulacja przerzutki", Price = 70.00M, EstimatedTime = 1.0 }
        //        );
        //        db.SaveChanges();
        //    }
        //}
    }
}
