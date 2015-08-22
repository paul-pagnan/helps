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

namespace helps.Service.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    //[Route("ConfirmEmail")]
    public class ConfirmEmailController : ApiController
    {
        public ApiServices Services { get; set; }

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
                return this.Request.CreateResponse(HttpStatusCode.OK, "Email confirmed successfully. Please login");
            }
            return this.Request.CreateResponse(HttpStatusCode.BadRequest, "User not found");
        }

    }
}
