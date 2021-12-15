using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace VendingMachineBackend.Providers
{
    public class OAuthCookieProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (context == null) throw new ArgumentException("context");
            var tokenCookie = context.OwinContext.Request.Cookies["BearerToken"];
            if (!string.IsNullOrEmpty(tokenCookie))
            {
                context.Token = tokenCookie;
                return Task.FromResult<object>(null);
            }
            return base.RequestToken(context);
        }
    }
}