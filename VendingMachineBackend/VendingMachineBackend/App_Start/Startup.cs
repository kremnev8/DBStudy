using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using VendingMachineBackend;
using VendingMachineBackend.Models;
using VendingMachineBackend.Providers;

[assembly: OwinStartup(typeof(Startup))]

namespace VendingMachineBackend
{
    
    public class Startup
    {
        public static JwtFormat jwtFormat;

        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(VendingBusinessContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat("http://localhost:5000")
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }
        
        private void ConfigureOAuthTokenConsumption(IAppBuilder app) {

            var issuer = "http://localhost:5000";
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);


            IIssuerSecurityTokenProvider issuerSecurity = new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret);
            
            jwtFormat = new JwtFormat(new TokenValidationParameters
            {
                ValidAudience = audienceId
            }, issuerSecurity);
            
            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    Provider = new OAuthCookieProvider(),
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new[]
                    {
                        issuerSecurity
                    }
                });

        }
        
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            ConfigureOAuthTokenConsumption(app);
            
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        }
    }
}