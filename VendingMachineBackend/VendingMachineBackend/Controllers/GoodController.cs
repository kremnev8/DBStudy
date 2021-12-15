using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VendingMachineBackend.Models;
using VendingMachineBackend.Util;
using VendingMachineBackend.ViewModel;

namespace VendingMachineBackend.Controllers
{
    [System.Web.Mvc.Authorize]
    public class GoodController : Controller
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        
        
        // GET /Good/
        public ActionResult Index()
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() == Permission.None)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            VendingBusinessContext context = VendingBusinessContext.Create();
            List<Good> goods = context.good.Include(good => good.Supplier).ToList();
            
            return View(goods);
        }
        
        // GET /Good/Detail?goodId=
        public ActionResult Detail(int goodId)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() == Permission.None)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            VendingBusinessContext context = VendingBusinessContext.Create();
            try
            {
                Good good = context.good.Include(g => g.Supplier).First(good1 => good1.GoodId == goodId);
                return View(good);
            }
            catch (InvalidOperationException e)
            {
                Response.Status = "404 NotFound";
                return View("NotFound");
            }
            
        }

        // GET /Good/Add
        public ActionResult Add()
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() == Permission.None || employee.getPermission() == Permission.Techician)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            return View();
        }

        // POST /Good/SaveNewGood
        [HttpPost]
        public ActionResult SaveNewGood(GoodAddViewModel model)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() == Permission.None || employee.getPermission() == Permission.Techician)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            if (!ModelState.IsValid) return View("Add", model);

            string iconPath = "";
            
            if (model.GoodIcon != null)
            {
                string fileSavePath = Server.MapPath("~/Images/" + model.GoodIcon.FileName);
                FileInfo file = new FileInfo(fileSavePath);
                if (!file.Exists)
                {
                    model.GoodIcon?.SaveAs(fileSavePath);
                    iconPath = model.GoodIcon.FileName;
                }
                else
                {
                    string salt = Helpers.RandomString(10);
                    iconPath = Path.GetFileNameWithoutExtension(model.GoodIcon.FileName) + salt + Path.GetExtension(model.GoodIcon.FileName);
                    string newSavePath = Server.MapPath("~/Images/" + iconPath);
                    model.GoodIcon?.SaveAs(newSavePath);
                }

            }


            Good good = new Good()
            {
                Name = model.Name,
                SupplierId = int.Parse(model.SuppliderId),
                SaleCost = model.SaleCost,
                PurchaseCost = model.PurchaseCost,
                IconPath = iconPath
            };
            VendingBusinessContext context = VendingBusinessContext.Create(); 
            context.good.Add(good);
            context.SaveChanges();

            return RedirectToAction("Index", "Good");
        }
        
    }
}