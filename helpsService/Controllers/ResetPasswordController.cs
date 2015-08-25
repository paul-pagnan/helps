using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Net.Http.Headers;
using helps.Service.DataObjects;
using helps.Service.Utils;
using helps.Service.Models;


namespace helps.Service.Controllers
{
    public class ResetPasswordController : ApiController
    {
        public ApiServices Services { get; set; }

        // GET api/ResetPassword

        public HttpResponseMessage Get(string Token)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("<html><body>Here is the password reset</body></html>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        // GET api/ForgotPassword
        public HttpResponseMessage Post(ForgotPasswordRequest request)
        {
            helpsContext context = new helpsContext();
            // Find the User with the token which was emailed to them
            User user = context.Users.Where(a => a.ForgotPasswordToken == request.ResetToken).SingleOrDefault();

            if (user != null)
            {
                if (request.Password != request.ConfirmPassword)
                {
                    // FAILED - Return Error
                    return null;
                }

                byte[] salt = LoginProviderUtil.generateSalt();
                user.Salt = salt;
                user.SaltedAndHashedPassword = LoginProviderUtil.hash(request.Password, salt);
                user.ForgotPasswordToken = Guid.NewGuid().ToString();

                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                //RENDER SUCCESS
            }

            //RENDER USER NOT FOUND
            return null;
        }

    }
}
