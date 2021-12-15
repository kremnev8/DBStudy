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
    public class DeliveryController : Controller
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
        // GET
        public ActionResult Index(int page = 1, int limit = 15)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() < Permission.Manager)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }

            var skip = limit * (page-1);

            VendingBusinessContext context = VendingBusinessContext.Create();
            int count = context.delivery.Count();
            int maxPage = (int)Math.Ceiling(count / (float) limit);
            
            List<Delivery> goods = context.delivery.OrderBy(d => d.DeliveryId).Skip(skip).Take(limit).ToList();

            ViewBag.page = page;
            ViewBag.limit = limit;
            ViewBag.maxPage = maxPage;
            return View(goods);
        }
        
        public ActionResult Detail(int deliveryId)
        {

            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() < Permission.Manager)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            VendingBusinessContext context = VendingBusinessContext.Create();
            try
            {
                Delivery machine = context.delivery
                    .Include(v => v.contents.Select(s => s.Good))
                    .First(d => d.DeliveryId == deliveryId);
                
                return View(machine); 
            }
            catch (InvalidOperationException e)
            {
                Response.Status = "404 NotFound";
                return View("NotFound");
            }
        }
        
    }
}