using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using VendingMachineBackend.Models;
using VendingMachineBackend.ViewModel;
#pragma warning disable 649
#pragma warning disable 414

namespace VendingMachineBackend.Controllers
{
    public class TokenData
    {
        public string access_token;
        public string token_type;
        public int expires_in;
    }
    
    public class AccountController : Controller
    {
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        
        public ActionResult Index()
        {
            return View();
        }
        
        //Return Register view
        public ActionResult Register()
        {
            return View();
        }
        

        //The form's data in Register view is posted to this method. 
        //We have binded the Register View with Register ViewModel, so we can accept object of Register class as parameter.
        //This object contains all the values entered in the form by the user.
        [HttpPost]
        public ActionResult SaveRegisterDetails(Register registerDetails)
        {
            if (!ModelState.IsValid) return View("Register", registerDetails);
            
            if (VendingBusinessContext.Create().employee.Any(e => e.Email == registerDetails.Email))
            {
                ModelState.AddModelError("", "User with that Email is already registered!");
                return View("Register", registerDetails);
            }
            
            Employee user = new Employee
            {
                FullName = registerDetails.FullName, 
                Email = registerDetails.Email, 
                Password = registerDetails.Password,
                PermissionId = 1
            };
            
            VendingBusinessContext context = VendingBusinessContext.Create();
            context.employee.Add(user);
            context.SaveChanges();
            return RedirectToAction("Index", "Account");
        }
        
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        
        
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(Login model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            HttpClient client = new HttpClient();
            Uri uri = new Uri("http://localhost:5000/api/token");
                
            var request = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = model.Email,
                ["password"] = model.Password
            };

            HttpResponseMessage response = await client.PostAsync(uri, new FormUrlEncodedContent(request));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream contentStream = await response.Content.ReadAsStreamAsync();
                
                StreamReader streamReader = new StreamReader(contentStream);
                JsonTextReader jsonReader = new JsonTextReader(streamReader);

                JsonSerializer serializer = new JsonSerializer();

                try
                {
                    TokenData data = serializer.Deserialize<TokenData>(jsonReader);

                    HttpCookie cookie = new HttpCookie("BearerToken", data.access_token)
                    {
                        Expires = DateTime.Now.AddDays(1)
                    };
                    
                    Response.Cookies.Add(cookie);
                    return RedirectToLocal(returnUrl);
                }
                catch(JsonReaderException)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }
            
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Account");
        }

        
        public ActionResult Logout()
        {
            if (Request.Cookies["BearerToken"] != null)
            {
                HttpCookie myCookie = new HttpCookie("BearerToken")
                {
                    Expires = DateTime.Now.AddDays(-1d)
                };
                Response.Cookies.Add(myCookie);
            }
            return RedirectToAction("Index", "Account");
        }
    }
}