using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VendingMachineBackend.Models;
using VendingMachineBackend.Util;
using VendingMachineBackend.ViewModel;

namespace VendingMachineBackend.Controllers
{
    [System.Web.Mvc.Authorize]
    public class MachineController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
 
        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }
 
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }
 
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;


        // GET
        public ActionResult Index()
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = UserManager.Users.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            VendingBusinessContext context = VendingBusinessContext.Create();
            List<VendingMachine> machines;

            switch (employee.getPermission())
            {
                case Permission.None:
                    Response.Status = "403 Forbidden";
                    return View("Forbidden");
                case Permission.Techician:
                    machines = context.vendingmachine
                        .Include(v => v.slots.Select(s => s.Good))
                        .Where(machine => machine.EmployeeId == employee.EmployeeId).ToList();
                    return View(machines);
                case Permission.Manager:
                case Permission.Admin:
                    machines = context.vendingmachine
                        .Include(v => v.slots.Select(s => s.Good)).ToList();
                    return View(machines);
            }

            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
        
        public ActionResult Detail(int machineId)
        {

            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = UserManager.Users.First(employee1 => employee1.Email.Equals(user.Identity.Name));

            if (employee.getPermission() == Permission.None)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            VendingBusinessContext context = VendingBusinessContext.Create();
            try
            {
                VendingMachine machine = context.vendingmachine
                    .Include(v => v.slots.Select(s => s.Good))
                    .First(vendingMachine => vendingMachine.MachineId == machineId);

                if (employee.getPermission() == Permission.Techician && 
                    machine.EmployeeId != employee.EmployeeId)
                {
                    Response.Status = "403 Forbidden";
                    return View("Forbidden");
                }
                
            

                return View(machine); 
            }
            catch (InvalidOperationException e)
            {
                Response.Status = "404 NotFound";
                return View("NotFound");
            }
        }

        public ActionResult Add()
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() != Permission.Manager && employee.getPermission() != Permission.Admin)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            return View();
        }

        public ActionResult SaveNewMachine(MachineAddViewModel model)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() != Permission.Manager && employee.getPermission() != Permission.Admin)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            if (!ModelState.IsValid) return View("Add", model);
            
            VendingMachine good = new VendingMachine()
            {
                Address = model.Address,
                EmployeeId = int.Parse(model.EmployeeId),
                AccessToken = ""
            };
            
            VendingBusinessContext context = VendingBusinessContext.Create(); 
            context.vendingmachine.Add(good);
            context.SaveChanges();

            return RedirectToAction("Index", "Machine");
        }
        

        
        // POST: /Machine/Generate
        [System.Web.Mvc.HttpPost]
        public ActionResult Generate(int machineId)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = UserManager.Users.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            VendingBusinessContext context = VendingBusinessContext.Create();
            try
            {
                VendingMachine machine = context.vendingmachine.First(machine1 => machine1.MachineId == machineId);
    
                if (machine.EmployeeId != employee.EmployeeId)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }
    
                if (string.IsNullOrEmpty(machine.AccessToken))
                {
                    string token = Helpers.RandomString(64);
                    machine.AccessToken = token;
                    context.SaveChanges();
                    return RedirectToAction("Index", "Machine");
                }
    
                return View("InvalidRequst");
            }
            catch (InvalidOperationException e)
            {
                Response.Status = "404 NotFound";
                return View("NotFound");
            }
        }
    }
}