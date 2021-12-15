using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using VendingMachineBackend.Models;
using VendingMachineBackend.Util;

namespace VendingMachineBackend.APIControllers
{
    public class MachineController : ApiController
    {
        // GET api/Machine/Get
        public IHttpActionResult Get()
        {
            if (this.CheckAuthorization(out VendingMachine machine, true))
            {
                return Ok(machine);
            }
            
            return StatusCode(HttpStatusCode.Unauthorized);
        }
    }
}