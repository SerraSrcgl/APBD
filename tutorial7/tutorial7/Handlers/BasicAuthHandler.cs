using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using tutorial7.Services;

namespace tutorial7.Handlers
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        IStudentsDbService _service;
        public BasicAuthHandler(
             IOptionsMonitor<AuthenticationSchemeOptions> options,
             ILoggerFactory logger,
             UrlEncoder encoder,
             ISystemClock clock ,   //lifetime of the token
            IStudentsDbService service
         ) : base(options, logger, encoder, clock)  //super in java
        {
            _service = service;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()//service i burayada koyabilirdik
        {

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing authorization header!!");
            }


            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]); //base64 olan heaeri aldik parse ettik 
            var credentialsBytes = Convert.FromBase64String(authHeader.Parameter);  //byte a cevirdik
            var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(":");  // aysenur:12345

            if (credentials.Length != 2)
            {
                return AuthenticateResult.Fail("Incorrect authorization header value");  //login ve password den baska bisey girmis mi diye bakiyorz
            }

            bool validation = _service.validationCredential(credentials[0], credentials[1]);


            if (validation == false)
            {
                return AuthenticateResult.Fail("Incorrect username or password.");
            }

            //basic info about user
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, credentials[0]),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "employee")

            };

            var identity = new ClaimsIdentity(claims, Scheme.Name); //passport
            var principal = new ClaimsPrincipal(identity); //wallet 
            var ticket = new AuthenticationTicket(principal, Scheme.Name);  //assigning ticket-> allow user to use spesific service

            return AuthenticateResult.Success(ticket);

        }
    }
}
