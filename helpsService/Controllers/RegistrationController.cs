using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Text.RegularExpressions;
using helps.Service.DataObjects;
using helps.Service.Models;
using helps.Service.Utils;

namespace helps.Service.Controllers
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
            else if (!Regex.IsMatch(registrationRequest.Email, "^[A-Za-z0-9._%+-]+@+(.*?.)?uts.edu.au$"))
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Email must be a UTS email address");
            }
            else if (registrationRequest.Password.Length < 8)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Password (at least 8 chars required)");
            }

            helpsDbContext context = new helpsDbContext();
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
                    FirstName = registrationRequest.FirstName,
                    LastName = registrationRequest.LastName,
                    StudentId = registrationRequest.StudentId,
                    Salt = salt,
                    Email = registrationRequest.Email,
                    Confirmed = false,
                    ConfirmToken = Guid.NewGuid().ToString(),
                    ForgotPasswordToken = Guid.NewGuid().ToString(),
                    ResetTokenSentAt = DateTime.Now,
                    SaltedAndHashedPassword = LoginProviderUtil.hash(registrationRequest.Password, salt)
                };

                var url = Request.RequestUri.GetLeftPart(UriPartial.Authority) + Url.Route("DefaultApi", new { controller = "ConfirmEmail", Token = newUser.ConfirmToken });
                EmailProviderUtil.SendConfirmationEmail(newUser, url);
                
                context.Users.Add(newUser);
                context.SaveChanges();
                return this.Request.CreateResponse(HttpStatusCode.Created);
            }
        }
    }
}
