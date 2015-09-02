using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using helps.Service.DataObjects;
using helps.Service.Models;
using helps.Service.Helpers;
using helps.Service.Utils;

namespace helps.Service.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    //[Route("ConfirmEmail")]
    public class ConfirmEmailController : ApiController
    {
        public ApiServices Services { get; set; }
        public HttpResponseMessage Get(string StudentId, string Resend)
        {
            helpsContext context = new helpsContext();
            // Find the User with the token which was emailed to them
            User user = context.Users.Where(a => a.StudentId == StudentId).SingleOrDefault();

            if (user != null)
            {
                if (user.Confirmed)
                    return this.Request.CreateResponse(HttpStatusCode.InternalServerError, "Email already confirmed");

                var url = Request.RequestUri.GetLeftPart(UriPartial.Authority) + Url.Route("DefaultApi", new { controller = "ConfirmEmail", Token = user.ConfirmToken });
                EmailProviderUtil.SendConfirmationEmail(user, url);

                //Return success
                return this.Request.CreateResponse(HttpStatusCode.OK, "Email confirmation sent");
            }
            return this.Request.CreateResponse(HttpStatusCode.NotFound, "User not found");
        }

        // GET api/ConfirmEmail
        public HttpResponseMessage Get(string Token)
        {
            
            helpsContext context = new helpsContext();
            // Find the User with the token which was emailed to them
            User user = context.Users.Where(a => a.ConfirmToken == Token).SingleOrDefault();

            if (user != null)
            {
                // Mark the user as confirmed
                user.Confirmed = true;
                // Update the database
                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                //Return success
                return ViewHelper.View("ConfirmEmail/Index", new { Message = "Email confirmed! Please navigate back to the app to log in" });
            }
            return ViewHelper.View("ConfirmEmail/Index", new { Message = "An error occured" });
        }
    }
}
