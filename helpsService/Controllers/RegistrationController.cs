using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Text.RegularExpressions;
using helpsService.DataObjects;
using helpsService.Models;
using helpsService.Utils;

namespace helpsService.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class RegistrationController : ApiController
    {
        public ApiServices Services { get; set; }

        // POST api/CustomRegistration
        public HttpResponseMessage Post(RegistrationRequest registrationRequest)
        {
            if (!Regex.IsMatch(registrationRequest.StudentId, "^[0-9]{8}$"))
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Student Id");
            }
            else if (registrationRequest.Password.Length < 8)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Password (at least 8 chars required)");
            }

            helpsContext context = new helpsContext();
            User account = context.Users.Where(a => a.StudentId == registrationRequest.StudentId).SingleOrDefault();
            if (account != null)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "That user already exists, please log in.");
            }
            else
            {
                byte[] salt = LoginProviderUtil.generateSalt();
                User newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    StudentId = registrationRequest.StudentId,
                    Salt = salt,
                    SaltedAndHashedPassword = LoginProviderUtil.hash(registrationRequest.Password, salt)
                };
                context.Users.Add(newUser);
                context.SaveChanges();
                return this.Request.CreateResponse(HttpStatusCode.Created);
            }
        }

    }
}
