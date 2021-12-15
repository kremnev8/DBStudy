using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using VendingMachineBackend.Models;
using VendingMachineBackend.Util;

namespace VendingMachineBackend.APIControllers
{
    public class PurchaseController : ApiController
    {
        
        // GET: api/Purchase/Get
        [HttpGet]
        public IHttpActionResult Get()
        {
            VendingBusinessContext context = VendingBusinessContext.Create();
            if (this.CheckAuthorization(out VendingMachine machine))
            {
                try { 
                    return Ok(context.purchase.Where(purchase =>  purchase.MachineId == machine.MachineId).ToArray());
                }
                catch (InvalidOperationException e)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }
        

        // GET: api/Purchase/Detail
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            VendingBusinessContext context = VendingBusinessContext.Create();
            if (this.CheckAuthorization(out VendingMachine machine))
            {
                try { 
                    return Ok(context.purchase.First(purchase => purchase.PurchaseId == id && purchase.MachineId == machine.MachineId));
                }
                catch (InvalidOperationException e)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }


        // POST: api/Purchase/Make
        [HttpPost]
        public HttpResponseMessage Make([FromBody] dynamic value)
        {
            VendingBusinessContext context = VendingBusinessContext.Create();

            if (this.CheckAuthorization(out VendingMachine machine))
            {
                try
                {
                    Purchase purchase = value.ToObject<Purchase>();
                    
                    VendingMachineSlot machineSlot = context.vendingmachineslot.First(slot =>
                        slot.MachineId == machine.MachineId && slot.GoodId == purchase.GoodId && slot.SlotPosition == purchase.SlotPosition);

                    if (machineSlot.GoodCount - purchase.GoodCount < 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Vending machine has not enough items!");
                    }

                    purchase.MachineId = machine.MachineId;
                    purchase.PurchaseTime = DateTime.Now;
                    
                    context.purchase.Add(purchase);
                    context.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.Created, "Success!");
                }
                catch (InvalidOperationException e)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not found vending machine slot with specified parameters");
                }
                catch (RuntimeBinderException e)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        "Missing parameters! Please ensure you have 'employeeToken' and 'delivery' parameters in your request.");
                }
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized, "Missing or invalid vending machine token!");
        }
    }
}