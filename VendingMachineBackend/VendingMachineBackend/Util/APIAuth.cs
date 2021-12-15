using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using VendingMachineBackend.Models;

namespace VendingMachineBackend.Util
{
    public static class APIAuth
    {
        public static bool CheckAuthorization(this ApiController controller, out VendingMachine result, bool includeSlots = false)
        {
            try
            {
                string token = controller.Request.Headers.Authorization.Parameter;
                IQueryable<VendingMachine> machines;
                if (!includeSlots)
                {
                    machines = VendingBusinessContext.Create().vendingmachine;
                }
                else
                {
                    machines = VendingBusinessContext.Create().vendingmachine.Include(v => v.slots);
                }

                result = machines.First(machine => machine.AccessToken.Equals(token));
                return true;
            }
            catch (InvalidOperationException e)
            {
                result = null;
                return false;
            }
            catch (NullReferenceException e)
            {
                result = null;
                return false;
            }
        }
    }
}