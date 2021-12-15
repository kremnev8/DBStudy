using System;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Owin.Security.Jwt;
using Thinktecture.IdentityModel.Extensions;
using VendingMachineBackend.Models;
using VendingMachineBackend.Util;

namespace VendingMachineBackend.APIControllers
{
    public class DeliveryController : ApiController
    {
        // GET: api/Delivery/Get
        [HttpGet]
        public IHttpActionResult Get()
        {
            VendingBusinessContext context = VendingBusinessContext.Create();
            if (this.CheckAuthorization(out VendingMachine machine))
            {
                try
                {
                    Delivery[] deliveries = context.delivery.Include(d => d.contents).Where(delivery => delivery.MachineId == machine.MachineId).ToArray();

                    return Ok(deliveries);
                }
                catch (InvalidOperationException e)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }


        // GET: api/Delivery/Detail
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            VendingBusinessContext context = VendingBusinessContext.Create();
            if (this.CheckAuthorization(out VendingMachine machine))
            {
                try
                {
                    Delivery delivery = context.delivery.Include(d => d.contents)
                        .First(delivery1 => delivery1.DeliveryId == id && delivery1.MachineId == machine.MachineId);
                    return Ok(delivery);
                }
                catch (InvalidOperationException e)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }


        // POST : api/Delivery/Make
        [HttpPost]
        public HttpResponseMessage Make([FromBody] dynamic value)
        {
            if (!this.CheckAuthorization(out VendingMachine machine))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Missing or invalid vending machine token!");
            }

            try
            {
                string employeeToken = value.employeeToken.Value;
                Delivery delivery = value.delivery.ToObject<Delivery>();

                ClaimsIdentity identity;

                try
                {
                    identity = Startup.jwtFormat.Unprotect(employeeToken).Identity;
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Missing or invalid employee token!");
                }

                Employee employee = VendingBusinessContext.Create().employee.First(employee1 => employee1.Email.Equals(identity.Name));

                if (machine.EmployeeId == employee.EmployeeId)
                {
                    delivery.EmployeeId = employee.EmployeeId;
                    delivery.MachineId = machine.MachineId;
                    delivery.DeliveryDate = DateTime.Now;

                    VendingBusinessContext context = VendingBusinessContext.Create();
                    context.delivery.Add(delivery);
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Created, "Success!");
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, "This employee can't make delivery to this vending machine!");
            }
            catch (RuntimeBinderException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    $"Missing parameters! Please ensure you have 'employeeToken' and 'delivery' parameters in your request.");
            }
            catch (InvalidOperationException e)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Missing or invalid employee token!");
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error!");
        }
    }
}