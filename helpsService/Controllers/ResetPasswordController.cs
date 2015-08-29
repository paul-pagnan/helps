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
using helps.Service.Helpers;


namespace helps.Service.Controllers
{
    public class ResetPasswordController : ApiController
    {
        // GET api/ResetPassword
        public HttpResponseMessage Get(string Token)
        {
            ResetPasswordRequest model = new ResetPasswordRequest();
            model.Errors = "8 characters minimum";
            model.ResetToken = Token;
            return ViewHelper.View("ResetPassword/Index", model);
        }

        // GET api/ForgotPassword
        public HttpResponseMessage Post(ResetPasswordRequest request)
        {
            helpsContext context = new helpsContext();
            // Find the User with the token which was emailed to them
            User user = context.Users.Where(a => a.ForgotPasswordToken == request.ResetToken).SingleOrDefault();

            if (user != null)
            {
                if (request.Password != request.ConfirmPassword)
                {
                    request.Errors = "Passwords do not match";
                    return ViewHelper.View("ResetPassword/Index", request);
                }

                byte[] salt = LoginProviderUtil.generateSalt();
                user.Salt = salt;
                user.SaltedAndHashedPassword = LoginProviderUtil.hash(request.Password, salt);
                user.ForgotPasswordToken = Guid.NewGuid().ToString();

                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return ViewHelper.View("ResetPassword/Success");
            }

            request.Errors = "An error occured";
            return ViewHelper.View("ResetPassword/Index", request);
        }

    }
}
