using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using VendingMachineBackend.Models;

namespace VendingMachineBackend.APIControllers
{
    public class EmployeeStripped
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public decimal Salary { get; set; }
        public int PermissionId { get; set; }
    }
    
    public class EmployeeController : ApiController
    {
        // GET api/Employee/Get
        public IHttpActionResult Get()
        {
            string employeeToken = Request.Headers.Authorization.Parameter;

            try
            {
                ClaimsIdentity identity = Startup.jwtFormat.Unprotect(employeeToken).Identity;
                Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(identity.Name));
                EmployeeStripped result = new EmployeeStripped()
                {
                    EmployeeId = employee.EmployeeId,
                    FullName = employee.FullName,
                    Salary = employee.Salary,
                    PermissionId = employee.PermissionId
                };
                
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }
        }
    }
}