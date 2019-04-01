using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SWZSR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Data
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext db = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            //ApplicationDbContext db = app.ApplicationServices.
            db.Database.Migrate();

            if (!db.Services.Any())
            {
                db.Services.AddRange(
                    new Service { Name = "Inny" },
                    new Service { Name = "Przegląd duży", Price = 199.00M, EstimatedTime = 4.0 },
                    new Service { Name = "Przegląd mały", Price = 99.00M, EstimatedTime = 3.0 },
                    new Service { Name = "Przegląd duży (24h)", Price = 239.00M, EstimatedTime = 1.0 },
                    new Service { Name = "Przegląd mały (24h)", Price = 129.00M, EstimatedTime = 1.0 },
                    new Service { Name = "Przegląd gwarancyjny", Price = 40.00M, EstimatedTime = 2.0 },
                    new Service { Name = "Centrowanie koła", Price = 35.00M, EstimatedTime = 2.0 },
                    new Service { Name = "Wymiana szprychy + centrowanie", Price = 40.00M, EstimatedTime = 2.0 },
                    new Service { Name = "Wymiana opony", Price = 15.00M, EstimatedTime = 1.0 },
                    new Service { Name = "Wymiana dętki", Price = 15.00M, EstimatedTime = 1.0 },
                    new Service { Name = "Wymiana dętki + dętka", Price = 36.00M, EstimatedTime = 1.0 },
                    new Service { Name = "Smarowanie łańcucha", Price = 0.00M, EstimatedTime = 1.0 },
                    new Service { Name = "Mycie i smarowanie łańcucha", Price = 30.00M, EstimatedTime = 1.0 },
                    new Service { Name = "Regulacja przerzutki", Price = 70.00M, EstimatedTime = 1.0 }
                );
                db.SaveChanges();
            }
            if (!db.Settings.Any())
            {
                db.Settings.AddRange(
                    new Setting { SettingId = 1, Name = "Nazwa serwisu", Value = "SWZSR Service", Key = "companyname" },
                    new Setting { SettingId = 2, Name = "Dane firmy", Value = "Sklep Rowerowy TWÓJSKLEP<br>ul. Bora-Komorowskiego 18A<br>03-982 Warszawa", Key = "companyaddress" },
                    new Setting { SettingId = 3, Name = "Adres email firmy", Value = "swzsr@kamilrojek.com", Key = "companyemail" },
                    new Setting { SettingId = 4, Name = "Opóźnienie w pracy serwisu", Value = "2", Key = "servicedelay" },
                    new Setting { SettingId = 5, Name = "Przyjmowanie zleceń", Value = "true", Key = "neworders" },
                    new Setting { SettingId = 6, Name = "SMSAPI: email użytkownika", Value = "tech@kamilrojek.com", Key = "smsapiemail" },
                    new Setting { SettingId = 7, Name = "SMSAPI: Hash hasła", Value = "ec54883cf7f0a123c01127f50b37aedd", Key = "smsapihash" }
                );
                db.SaveChanges();
            }

        }
    }
}
