using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SWZSR.Data;
using SWZSR.Infrastructure;
using SWZSR.Infrastructure.Alerts;
using SWZSR.Models;

namespace SWZSR.Controllers
{
    public class HomeController : Controller
    {
        private static IHostingEnvironment _env;
        private readonly ApplicationDbContext db;
        private AlertService _alertService { get; }

        public HomeController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context, AlertService alertService)
        {
            _env = hostingEnvironment;
            db = context;
            _alertService = alertService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return RedirectToAction("MyAccount", "Account");               
        }

        public IActionResult StaticContent(string viewname)
        {
            return View(viewname);
        }

        public IActionResult AccessDenied()
        {
            _alertService.Danger("Nie posiadasz wymaganych uprawnień.");
            return View();
        }

        public IActionResult Error()
        {
            _alertService.Danger("Nie znaleziono podanej strony. <a href=\"/Home/Index\">Wróć na stronę główną</a>");
            return View();
        }
    }
}
