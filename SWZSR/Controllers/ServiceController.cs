using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWZSR.Data;
using SWZSR.Infrastructure.Alerts;
using SWZSR.Models;

namespace SWZSR.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private AlertService _alertService { get; }
        private readonly ApplicationDbContext _db;

        public ServiceController(ApplicationDbContext context, AlertService alertService)
        {
            _db = context;
            _alertService = alertService;
        }

        public IActionResult AllServices()
        {
            var servicesList = _db.Services.ToList();

            return View(servicesList);
        }

        // GET: /Service/EditService
        public IActionResult EditService(int serviceid)
        {
            var service = _db.Services.Find(serviceid);
            if(service != null)
            {
                var model = new Service
                {
                    ServiceId = service.ServiceId,
                    Price = service.Price,
                    Name = service.Name,
                    EstimatedTime = service.EstimatedTime
                };

                return View(model);
            }
            else
            {
                _alertService.Danger("Taka pozycja nie istnieje.");
                return RedirectToAction("AllServices");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(Service model)
        {
            if (ModelState.IsValid)
            {
                var service = await _db.Services.FindAsync(model.ServiceId);
                if(service != null)
                {
                    service.Name = model.Name;
                    service.Price = model.Price;
                    service.EstimatedTime = model.EstimatedTime;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    _alertService.Danger("Taka pozycja nie istnieje.");
                    return RedirectToAction("AllServices");
                }
            }
            else
            {
                _alertService.Danger("Błędnie wypełniono formularz.");
                return View(model);
            }

            _alertService.Success("Pomyślnie zmodyfikowano usługę.");
            return RedirectToAction("AllServices");
        }

        // GET: /Service/AddService
        public IActionResult AddService()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddService(Service model)
        {
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Name = model.Name,
                    EstimatedTime = model.EstimatedTime,
                    Price = model.Price
                };
                _db.Services.Add(service);
                await _db.SaveChangesAsync();
            }
            else
            {
                _alertService.Danger("Błędnie wypełniono formularz.");
                return View(model);
            }
            _alertService.Success("Pomyślnie dodano nową usługę.");
            return RedirectToAction("AllServices");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int serviceid)
        {
            var service = await _db.Services.FindAsync(serviceid);
            if (service != null)
            {
                _db.Services.Remove(service);
                await _db.SaveChangesAsync();
                _alertService.Success("Usługa została usunięta.");
                return RedirectToAction("AllServices");
            }
            else
            {
                _alertService.Danger("Taka pozycja nie istnieje.");
                return RedirectToAction("AllServices");
            }
        }

    }
}