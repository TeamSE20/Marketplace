// OrderController.cs
using IdentityAppCourse2022.Data;
using IdentityAppCourse2022.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System;
using System.Linq;

namespace IdentityAppCourse2022.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Statistics()
        {
            var orderQuery = _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.IsClosed);

            var totalOrders = orderQuery.Count();

            var totalRevenue = orderQuery
                .SelectMany(o => o.OrderItems)
                .Sum(oi => oi.Product.Price * oi.Quantity);

            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            var bestSellingProduct = GetBestSellingProduct();

            var statistics = new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                AverageOrderValue = averageOrderValue,
                BestSellingProduct = bestSellingProduct
            };

            return View(statistics);
        }

        private string GetBestSellingProduct()
        {

            var bestSellingProduct = _db.OrderItems
            .GroupBy(oi => oi.Product.Name)
            .Select(group => new
            {
                ProductName = group.Key,
                QuantitySum = group.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(result => result.QuantitySum)
            .FirstOrDefault()?.ProductName;

            return bestSellingProduct ?? "No sales yet"; // Если нет продаж, возвращаем соответствующее сообщение
        }

        [HttpPost]
        public IActionResult Create(string productId)
        {
            bool orderExcsist = false;

            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                // Handle the case where the user is not authenticated.
                return RedirectToAction("Login", "Account");
            }

            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.IsClosed == false);
            if (order == null)
            {
                // Create a new order
                order = new Order
                {
                    ClientId = user.Id,
                    OrderDate = DateTime.Now,
                    OrderItems = new System.Collections.Generic.List<OrderItem>()
                };
            }else
            {
                orderExcsist = true;
            }

            // Find the product to add to the order
            var product = _db.Product.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
                if(orderItem == null)
                {
                    orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1 // You can adjust the quantity as needed
                    };
                } else
                {
                    orderItem.Quantity += 1;
                }
                // Add the product to the order

                order.OrderItems.Add(orderItem);

                // Save the order to the database
                if(orderExcsist)
                {
                    _db.Orders.Update(order);
                }else
                {
                    _db.Orders.Add(order);
                }
                _db.SaveChanges();
            }

            // Redirect to the cart or wherever you want
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string productId)
        {

            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                // Handle the case where the user is not authenticated.
                return RedirectToAction("Login", "Account");
            }

            var order = _db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.IsClosed == false);
            if (order != null)
            {
                // Find the product to add to the order
                var product = _db.Product.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    var orderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
                    if (orderItem != null)
                    {
                        if(orderItem.Quantity > 1)
                        {
                            orderItem.Quantity -= 1;
                        }else
                        {
                            order.OrderItems.Remove(orderItem);
                        }
                    }
                    _db.SaveChanges();
                }
            }

            // Redirect to the cart or wherever you want
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            // Retrieve the user's orders from the database
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                // Handle the case where the user is not authenticated.
                return RedirectToAction("Login", "Account");
            }

            var orders = _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.ClientId == user.Id && !o.IsClosed)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        public IActionResult OrderHistoryClient()
        {
            var user = _userManager.GetUserAsync(User).Result;

            return View(_db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.IsClosed == true && o.ClientId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList());
        }

        public IActionResult OrderHistory()
        {
            return View(_db.Orders.Include(o => o.Client).Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Where(o => o.IsClosed == true).OrderByDescending(o => o.OrderDate).ToList());
        }

        public IActionResult OrderConfirmation()
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());

            if(session.PaymentStatus.Equals("paid"))
            {
                var transaction = session.PaymentIntentId.ToString();
                var user = _userManager.GetUserAsync(User).Result;

                var orders = _db.Orders
                    .Where(o => o.ClientId == user.Id && !o.IsClosed)
                    .First();
                orders.IsClosed = true;
                _db.Orders.Update(orders);
                _db.SaveChanges();

                return View("Success");
            }
            return View("Login");
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult CheckOut()
        {
            var domain = "https://localhost:44395/";

            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Order/OrderConfirmation",
                CancelUrl = domain + "Order/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            var user = _userManager.GetUserAsync(User).Result;

            var orders = _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.ClientId == user.Id && !o.IsClosed)
                .OrderByDescending(o => o.OrderDate)
                .First();

            foreach(var item in orders.OrderItems)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * item.Quantity),
                        Currency = "kgs",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        }
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(sessionListItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            TempData["Session"] = session.Id;

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

    }
}
