using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWZSR.Data;
using SWZSR.Infrastructure;
using SWZSR.Infrastructure.Alerts;
using SWZSR.Models;
using SWZSR.ViewModels;

namespace SWZSR.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private NotificationManager _notification;
        private static IHostingEnvironment _env;
        private AlertService _alertService { get; }

        public OrderController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, AlertService alertService, UserManager<ApplicationUser> userManager, NotificationManager notification)
        {
            _db = context;
            _env = hostingEnvironment;
            _alertService = alertService;
            _userManager = userManager;
            _notification = notification;
        }

        // GET: Order/NewOrder
        public async Task<IActionResult> NewOrder()
        {
            DateTime today = DateTime.Now;
            ViewBag.availableDates = new List<DateTime>();
            for (int i = 1; i <= 10; i++)
                ViewBag.availableDates.Add(today.AddDays(i));
            var services = _db.Services.ToList();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var order = new Order
            {
                PhoneNumber = user.PhoneNumber,
                User = user,
                UserId = user.Id
            };

            var model = new OrderViewModel
            {
                Services = services,
                Order = order
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewOrder(OrderViewModel model, IFormFile itemPhoto)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            model.Order.UserId = user.Id;
            int orderId = 0;
            if (ModelState.IsValid && model.Order.OrderItems[0].OrderItemServices.Count() > 0)
            {
                string itemPhotoGeneratedUrl = null;

                if (!(itemPhoto == null || itemPhoto.Length == 0))
                {
                    itemPhotoGeneratedUrl = Path.Combine("uploads", DateTime.Now.ToString("yyyyMMddTHHmmss_") + itemPhoto.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", itemPhotoGeneratedUrl);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await itemPhoto.CopyToAsync(stream);
                    }
                }

                var order = new Order
                {
                    DateCreated = DateTime.Now,
                    DateDelivered = model.Order.DateDelivered,
                    DateCompleted = model.Order.DateCompleted,
                    PhoneNumber = model.Order.PhoneNumber,
                    OrderState = OrderState.New,
                    Comment = model.Order.Comment,
                    User = user,
                    UserId = user.Id
                };
                await _db.Orders.AddAsync(order);

                orderId = order.OrderId;

                decimal totalPrice = 0;

                foreach (var bike in model.Order.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = orderId,
                        ItemPhoto = itemPhotoGeneratedUrl,
                        ItemModel = bike.ItemModel,
                        ItemColour = bike.ItemColour,
                        Comment = bike.Comment
                    };
                    await _db.OrderItems.AddAsync(orderItem);
                    
                    var orderItemId = orderItem.OrderItemId;
                    foreach (var service in bike.OrderItemServices)
                    {
                        var orderItemService = new OrderItemService
                        {
                            OrderItemId = orderItemId,
                            ServiceId = service.ServiceId,
                            UnitPrice = service.UnitPrice
                        };
                        await _db.OrderItemServices.AddAsync(orderItemService);
                        
                        totalPrice += service.UnitPrice;
                    }
                }
                order.TotalPrice = totalPrice;
                await _db.SaveChangesAsync();
            }
            else
            {
                _alertService.Danger("Błędnie wypełniono formularz.");
                DateTime today = DateTime.Now;
                ViewBag.availableDates = new List<DateTime>();
                for (int i = 1; i <= 10; i++)
                    ViewBag.availableDates.Add(today.AddDays(i));
                var services = _db.Services.ToList();
                model.Services = services;
                return View(model);
            }

            _alertService.Success("Zlecenie serwisowe zostało dodane!");
            await _notification.SendEmail(MailType.NewOrder, user.Email, _env, orderId);
            return RedirectToAction("MyOrders");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> NextOrderStatus(int orderId, string returnKey = null)
        {
            var order = await _db.Orders.FindAsync(orderId);
            order.OrderState++;
            if (order.OrderState == OrderState.Accepted)
            {
                order.DateDelivered = DateTime.Now;
            }
            await _db.SaveChangesAsync();

            _alertService.Success("Pomyślnie zmieniono status zlecenia nr " + orderId + ".", true);
            if (order.OrderState == OrderState.InProgress)
            {
                order.DateCompleted = DateTime.Now;
                await _db.SaveChangesAsync();
                await _notification.SendEmail(MailType.StartedOrder, order.User.Email, _env, orderId);
            }
            if (order.OrderState == OrderState.Completed)
            {
                await _notification.SendEmail(MailType.FinishedOrder, order.User.Email, _env, orderId, order.TotalPrice);
                _notification.SendSMS(order.PhoneNumber, orderId, order.TotalPrice);
            }

            return RedirectToAction("AllOrders", new { key = returnKey });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> PreviousOrderStatus(int orderId, string returnKey = null)
        {
            var order = await _db.Orders.FindAsync(orderId);
            order.OrderState--;
            await _db.SaveChangesAsync();

            _alertService.Warning("Pomyślnie zmieniono status zlecenia nr " + orderId + " na poprzedni.", true);

            return RedirectToAction("AllOrders", new { key = returnKey });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int orderId, string returnKey = null)
        {
            var order = await _db.Orders.FindAsync(orderId);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            _alertService.Success("Usunięto zlecenie nr " + orderId + ".", true);

            return RedirectToAction("AllOrders", new { key = returnKey });
        }

        // GET: Order/EditOrder/
        public IActionResult EditOrder(int orderId)
        {
            var order = _db.Orders.Find(orderId);
            var services = _db.Services.ToList();
            var model = new OrderViewModel
            {
                Order = order,
                Services = services
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditOrder(OrderViewModel model)
        {
            var order = await _db.Orders.FindAsync(model.Order.OrderId);
            order.PhoneNumber = model.Order.PhoneNumber;
            order.Comment = model.Order.Comment;

            decimal totalPrice = 0;

            int index = 0;
            foreach (var bike in order.OrderItems)
            {
                bike.ItemModel = model.Order.OrderItems[index].ItemModel;
                bike.ItemColour = model.Order.OrderItems[index].ItemColour;
                bike.Comment = model.Order.OrderItems[index].Comment;

                bike.OrderItemServices.RemoveAll(o => o.OrderItemId == bike.OrderItemId);

                foreach (var service in model.Order.OrderItems[index].OrderItemServices)
                {
                    var orderItemService = new OrderItemService
                    {
                        OrderItemId = bike.OrderItemId,
                        ServiceId = service.ServiceId,
                        UnitPrice = service.UnitPrice
                    };
                    await _db.OrderItemServices.AddAsync(orderItemService);

                    totalPrice += service.UnitPrice;
                }
                index++;
            }
            order.TotalPrice = totalPrice;
            await _db.SaveChangesAsync();

            _alertService.Success("Zmieniono zlecenie!");

            return RedirectToAction("AllOrders");
        }

        public IActionResult MyOrders()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var orderList = _db.Orders.Where(o => o.UserId == userId).ToList();
            //var model = new OrderListViewModel
            //{
            //    Orders = orderList
            //};

            return View(orderList);
        }

        public IActionResult SeeOrder(int orderId)
        {
            var order = _db.Orders.Find(orderId);
            return View(order);
        }

        [Authorize(Roles = "Admin, Mechanic")]
        public async Task <IActionResult> AllOrders(string key = null)
        {
            List<Order> orders = new List<Order>();
            switch (key)
            {
                case null:
                    orders = await _db.Orders.OrderByDescending(o => o.DateDelivered).ToListAsync();
                    break;
                case "new":
                    orders = await _db.Orders.Where(o => o.OrderState == OrderState.New).OrderByDescending(o => o.DateDelivered).ToListAsync();
                    break;
                case "accepted":
                    orders = await _db.Orders.Where(o => o.OrderState == OrderState.Accepted).OrderByDescending(o => o.DateDelivered).ToListAsync();
                    break;
                case "inprogress":
                    orders = await _db.Orders.Where(o => o.OrderState == OrderState.InProgress).OrderByDescending(o => o.DateDelivered).ToListAsync();
                    break;
                case "completed":
                    orders = await _db.Orders.Where(o => o.OrderState == OrderState.Completed).OrderByDescending(o => o.DateDelivered).ToListAsync();
                    break;
            }
            ViewData["ReturnKey"] = key;
            return View(orders);
        }
    }
}