using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Security.Claims;
using helpsService.DataObjects;
using helpsService.Models;
using helpsService.Utils;

namespace helpsService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class SignInController : ApiController
    {
        public ApiServices Services { get; set; }
        public IServiceTokenHandler handler { get; set; }

        // POST api/SignIn
        public HttpResponseMessage Post(LoginRequest loginRequest)
        {
            helpsContext context = new helpsContext();
            User account = context.Users
                .Where(a => a.StudentId == loginRequest.StudentId).SingleOrDefault();
            if (account != null)
            {
                byte[] incoming = LoginProviderUtil
                    .hash(loginRequest.Password, account.Salt);

                if (LoginProviderUtil.slowEquals(incoming, account.SaltedAndHashedPassword))
                {
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity();
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginRequest.StudentId));
                    LoginResult loginResult = new helpsLoginProvider(handler).CreateLoginResult(claimsIdentity, Services.Settings.MasterKey);
                    var customLoginResult = new helpsLoginResult()
                    {
                        UserId = loginResult.User.UserId,
                        MobileServiceAuthenticationToken = loginResult.AuthenticationToken
                    };
                    return this.Request.CreateResponse(HttpStatusCode.OK, customLoginResult);
                }
            }
            return this.Request.CreateResponse(HttpStatusCode.Unauthorized,
                "Invalid username or password");
        }
    }
}
