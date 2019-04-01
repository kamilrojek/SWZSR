using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SWZSR.Data;
using SWZSR.Infrastructure.Alerts;
using SWZSR.Models;
using SWZSR.ViewModels;

namespace SWZSR.Controllers
{
    [Authorize(Roles = "Admin, Mechanic")]
    public class WorkshopController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private static IHostingEnvironment _env;
        private AlertService _alertService { get; }

        public WorkshopController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, AlertService alertService, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _env = hostingEnvironment;
            _alertService = alertService;
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public IActionResult ToDoList()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOrdersToCalendar(string start, string end)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime startDate = DateTime.ParseExact(start, "yyyy-MM-dd", provider);
            DateTime endDate = DateTime.ParseExact(end, "yyyy-MM-dd", provider);

            var orderList = _db.Orders.Where(o => o.DateDelivered >= startDate && o.DateDelivered <= endDate).ToList();
            List<WorkshopCalendarViewModel> ordersFormatted = new List<WorkshopCalendarViewModel>();
            foreach(var ord in orderList)
            {
                string colour = null, textcolour = null;

                    switch (ord.OrderState)
                    {
                        case OrderState.New:
                            colour = "white";
                            textcolour = "black";
                            break;
                        case OrderState.Accepted:
                            colour = "red";
                            textcolour = "white";
                            break;
                        case OrderState.InProgress:
                            colour = "yellow";
                            textcolour = "black";
                            break;
                        case OrderState.Completed:
                            colour = "green";
                            textcolour = "white";
                            break;
                        case OrderState.Received:
                            colour = "black";
                            textcolour = "white";
                            break;
                    }

                ordersFormatted.Add(new WorkshopCalendarViewModel
                {
                    Title = ord.OrderItems[0].ItemModel,
                    Start = ord.DateDelivered.ToString("yyyy-MM-dd"),
                    Url = "/Order/SeeOrder?orderId=" + ord.OrderId,
                    Color = colour,
                    TextColor = textcolour
                });
            }

            return Json(ordersFormatted);
        }

        public async Task<IActionResult> TakeOrderItem(int orderItemId, int orderId)
        {
            var order = await _db.Orders.FindAsync(orderId);
            order.OrderState++;
            var orderItem = await _db.OrderItems.FindAsync(orderItemId);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            orderItem.MechanicId = user.Id;
            await _db.SaveChangesAsync();

            return RedirectToAction("EditOrderItem", new { OrderItemId = orderItemId });
        }

        // GET: Workshop/EditOrderItem/
        public IActionResult EditOrderItem(int orderItemId)
        {
            var orderItem = _db.OrderItems.Find(orderItemId);
            var services = _db.Services.ToList();
            var model = new WorkshopViewModel
            {
                OrderItem = orderItem,
                Services = services
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveOrderItem(WorkshopViewModel model)
        {
            var orderItem = await _db.OrderItems.FindAsync(model.OrderItem.OrderItemId);

            decimal totalPrice = 0;
            orderItem.Comment = model.OrderItem.Comment;

            orderItem.OrderItemServices.RemoveAll(o => o.OrderItemId == orderItem.OrderItemId);

            foreach (var service in model.OrderItem.OrderItemServices)
            {
                var orderItemService = new OrderItemService
                {
                    OrderItemId = orderItem.OrderItemId,
                    ServiceId = service.ServiceId,
                    UnitPrice = service.UnitPrice
                };
                await _db.OrderItemServices.AddAsync(orderItemService);

                totalPrice += service.UnitPrice;
            }

            orderItem.Order.TotalPrice = totalPrice;
            await _db.SaveChangesAsync();

            _alertService.Success("Zapisano naprawę roweru.");

            return RedirectToAction("AllOrders", "Order", new { key = "inprogress" });
            }
    }
}