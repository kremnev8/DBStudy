using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Owin.Security;
using VendingMachineBackend.Models;
using VendingMachineBackend.ViewModel;

namespace VendingMachineBackend.Controllers
{
    public class EmployeeController : Controller
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
        // GET
        public ActionResult Index()
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() != Permission.Manager && employee.getPermission() != Permission.Admin)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
                
            VendingBusinessContext context = VendingBusinessContext.Create();
            List<Employee> goods = context.employee.ToList();
            
            return View(goods);
            
        }

        public ActionResult Detail(int employeeId)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (employee.getPermission() != Permission.Manager && employee.getPermission() != Permission.Admin)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }
            
            Employee employee2 = VendingBusinessContext.Create().employee.First(employee1 => employee1.EmployeeId == employeeId);
            ModifyEmployeeModel model = new ModifyEmployeeModel()
            {
                Email = employee2.Email,
                Salary = employee2.Salary,
                EmployeeId = employeeId,
                FullName = employee2.FullName,
                PermissionId = employee2.PermissionId.ToString()
            };
            
            return View(model);
        }

        public ActionResult ModifyData(ModifyEmployeeModel model, int employeeId)
        {
            ClaimsPrincipal user = AuthenticationManager.User;
            Employee requestUser = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(user.Identity.Name));
            
            if (requestUser.getPermission() == Permission.None || requestUser.getPermission() == Permission.Techician)
            {
                Response.Status = "403 Forbidden";
                return View("Forbidden");
            }

            if (requestUser.EmployeeId == employeeId)
            {
                Response.Status = "400 BadRequest";
                return View("BadRequest");
            }
            
            RouteValueDictionary dictionary = new RouteValueDictionary {{"employeeId", employeeId}};
            if (!ModelState.IsValid) return RedirectToAction("Detail", "Employee", dictionary);

            VendingBusinessContext context = VendingBusinessContext.Create();
            Employee employee = context.employee.First(employee2 => employee2.EmployeeId == employeeId);
            
            employee.Salary = model.Salary;
            employee.PermissionId = int.Parse(model.PermissionId);
            context.SaveChanges();

            return RedirectToAction("Detail", "Employee", dictionary);
        }
    }
}