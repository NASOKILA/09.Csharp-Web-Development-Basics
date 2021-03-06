﻿namespace HTTPServer.ByTheCakeApplication.Controllers
{
    using HTTPServer.ByTheCakeApplication.Data;
    using HTTPServer.ByTheCakeApplication.Models;
    using HTTPServer.Server.Http;
    using Infrastructure;
    using Server.Http.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HomeController : Controller
    {
        public IHttpResponse Index(IHttpRequest req) {

            var currentUserId = req.Session.Get<int>(SessionStore.CurrentUserKey);

            using (var context = new ByTheCakeContext())
            {
                User currentUser = context.Users.Find(currentUserId);

                if (currentUser != null)
                {
                    this.ViewData["username"] = currentUser.Username;
                }
            }

            return this.FileViewResponse(@"home\index");
        }

        public IHttpResponse About(IHttpRequest req)
        {
            var currentUserId = req.Session.Get<int>(SessionStore.CurrentUserKey);

            using (var context = new ByTheCakeContext())
            {
                User currentUser = context.Users.Find(currentUserId);

                if (currentUser != null)
                {
                    this.ViewData["name"] = currentUser.Name;
                }
            }

            return this.FileViewResponse(@"home\about");
        }

        public IHttpResponse Profile(IHttpRequest req)
        {
            var currentUserId = req.Session.Get<int>(SessionStore.CurrentUserKey);

            using (var context = new ByTheCakeContext())
            {
                User currentUser = context.Users.Find(currentUserId);

                int ordersCount = context.Orders
                    .Where(o => o.UserId == currentUserId)
                    .Count();

                if (currentUser != null)
                {
                    this.ViewData["name"] = currentUser.Name;
                    this.ViewData["dateOfRegistration"] = currentUser.DateOfRegistration.ToString("dd-MM-yyy");
                    this.ViewData["ordersCount"] = ordersCount.ToString();
                }
            }

            return this.FileViewResponse(@"home\profile");
        }

        public IHttpResponse Orders(IHttpRequest req) {

            var currentUserId = req.Session.Get<int>(SessionStore.CurrentUserKey);

            string result = "No Orders Created";

            using (var context = new ByTheCakeContext())
            {
                var orders = context
                    .Orders.Where(o => o.UserId == currentUserId)
                    .OrderByDescending(o => o.DateOfCreation)
                    .ToList();
                
                if (orders.Any()){
                   result = "";
                }

                for (int i = 0; i < orders.Count;i++)
                {
                    var order = orders[i];
                    List<Product> orderProducts = context.ProductOrders
                        .Where(po => po.ProductId == order.Id)
                        .Select(po => po.Product)
                        .ToList();

                    int num = i + 1;
                    decimal sum = orderProducts.Sum(op => op.Price);
                    result += $@"<tr><th scope=""row"">{num}</th><td><a href=""/orderDetails/{order.Id}"">{order.Id}</a></td><td>{order.DateOfCreation.ToString("dd-MM-yyyy")}</td><td>${sum}</td></tr>";
                }
            }
            
            this.ViewData["result"] = result;

            return this.FileViewResponse(@"home\orders");
        }
    }
}
