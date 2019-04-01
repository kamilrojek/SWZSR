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
    public class AdminController : Controller
    {
        private AlertService _alertService { get; }
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext context, AlertService alertService)
        {
            _db = context;
            _alertService = alertService;
        }

        public IActionResult Settings()
        {
            var settingList = _db.Settings.ToList();

            return View(settingList);
        }

        // GET: /Admin/UpdateSetting
        public IActionResult UpdateSetting(int settingid)
        {
            var setting = _db.Settings.Find(settingid);
            var model = new Setting
            {
                SettingId = setting.SettingId,
                Key = setting.Key,
                Name = setting.Name,
                Value = setting.Value
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSetting(Setting model)
        {
            var setting = _db.Settings.Find(model.SettingId);
            setting.Value = model.Value;
            _db.SaveChanges();

            _alertService.Success("Zaktualizowano ustawienie: " + model.Name + ".");
            return RedirectToAction("Settings");
        }
    }
}