using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using VendingMachineBackend.Models;

namespace VendingMachineBackend.Controllers
{
    public class PurchaseController : Controller
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
        // GET
        public ActionResult Index(int page = 1, int limit = 15)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() != Permission.Manager && employee.getPermission() != Permission.Admin)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }

            var skip = limit * (page-1);

            VendingBusinessContext context = VendingBusinessContext.Create();
            int count = context.purchase.Count();
            int maxPage = (int)Math.Ceiling(count / (float) limit);
            
            List<Purchase> goods = context.purchase.Include(p => p.Good).OrderBy(purchase => purchase.PurchaseId).Skip(skip).Take(limit).ToList();

            ViewBag.page = page;
            ViewBag.limit = limit;
            ViewBag.maxPage = maxPage;
            return View(goods);
        }
    }
}