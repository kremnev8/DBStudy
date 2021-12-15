using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VendingMachineBackend.Models;
using VendingMachineBackend.Util;

namespace VendingMachineBackend.APIControllers
{
    public class GoodController : ApiController
    {
        // GET: api/Good/Get
        [HttpGet]
        public IHttpActionResult Get()
        {
            VendingBusinessContext context = VendingBusinessContext.Create();
            if (this.CheckAuthorization(out VendingMachine _))
            {
                Good[] deliveries = context.good.ToArray();

                return Ok(deliveries);
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }


        // GET: api/Good/Detail
        [HttpGet]
        public IHttpActionResult Detail(int id)
        {
            VendingBusinessContext context = VendingBusinessContext.Create();
            if (this.CheckAuthorization(out VendingMachine _))
            {
                try
                {
                    Good delivery = context.good.First(g => g.GoodId == id);
                    return Ok(delivery);
                }
                catch (InvalidOperationException e)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }
    }
}