using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using helps.Service.Models;
using helps.Service.DataObjects;
using helps.Service.Utils;
using System.Net.Http.Headers;

namespace helps.Service.Controllers
{
    public class ForgotPasswordController : ApiController
    {
        public ApiServices Services { get; set; }

        [AllowAnonymous]
        public HttpResponseMessage Get(string studentId)
        {
            helpsContext context = new helpsContext();
            User user = context.Users.Where(a => a.StudentId == studentId).SingleOrDefault();

            if (user != null)
            {
                // Found the user
                user.ResetTokenSentAt = DateTime.Now;
            
                var url = Request.RequestUri.GetLeftPart(UriPartial.Authority) + Url.Route("DefaultApi", new { controller = "ResetPassword", Token = user.ForgotPasswordToken });
                EmailProviderUtil.SendPasswordResetEmail(user, url);

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            return this.Request.CreateResponse(HttpStatusCode.NotFound, "User not found");
        }
    }
}
