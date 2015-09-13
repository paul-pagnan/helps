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
    public class CompleteSetupController : ApiController
    {
        public ApiServices Services { get; set; }
        public HttpResponseMessage Get(string StudentId)
        {
            helpsDbContext context = new helpsDbContext();
            // Find the User with the token which was emailed to them
            User user = context.Users.Where(a => a.StudentId == StudentId).SingleOrDefault();
            if (user != null)
            {
                user.HasLoggedIn = true;
                context.SaveChanges();
                return this.Request.CreateResponse(HttpStatusCode.OK, "Success");
            }
            return this.Request.CreateResponse(HttpStatusCode.NotFound, "User not found");
        }
    }
}
