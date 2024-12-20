﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WebAPIProject.Repos;

namespace WebAPIProject.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly LearndataContext context;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,LearndataContext context) : base(options, logger, encoder, clock)
        {
            this.context = context;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No header found");
            }
            var headervalue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if(headervalue != null)
            {
                var bytes = Convert.FromBase64String(headervalue.Parameter);
                string credentials = Encoding.UTF8.GetString(bytes);
                string[] array = credentials.Split(':');
                string username = array[0];
                string password = array[1];
                var user = await this.context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Password == password);
                if(user != null)
                {
                    var claim = new[] { new Claim(ClaimTypes.Name, user.Name) };
                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("UnAuthorized");
                }
            }
            else
            {
                return AuthenticateResult.Fail("Empty Header");
            }
        }

    }
}
