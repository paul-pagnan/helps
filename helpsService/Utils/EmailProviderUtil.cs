using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using helps.Service.DataObjects;
using helps.Service;
using helps.Service.Controllers;
using System.Net.Http;
using System.Web.Http;
using RazorEngine;
using System.IO;
using System.Web.Configuration;
using System.Reflection;
using System.Text;

using helps.Service.Mail.ViewModels;
using RazorEngine.Configuration;
using RazorEngine.Text;
using RazorEngine.Templating;

namespace helps.Service.Utils
{
    public static class EmailProviderUtil 
    {
        private static string FromAddress = "utshelps25@gmail.com";
        private static string UserName = "utshelps25@gmail.com";
        private static string Password = "Password!23";
        private static string Host = "smtp.gmail.com";
        private static string templateFolderPath = Path.Combine(WebConfigurationManager.AppSettings["AppDir"], "Mail", "Templates");


        public static void SendConfirmationEmail(User user, string url)
        {
            SendNameUrlEmail("ConfirmEmail.cshtml", user, url, "UTS HELPS - Confirm Your Email");
        }
       
        public static void SendPasswordResetEmail(User user, string url)
        {
            SendNameUrlEmail("ResetPassword.cshtml", user, url, "UTS HELPS - Password Reset");
        }

        #region helpers
        private static void Send(MailMessage message)
        {
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(UserName, Password);
            client.Host = Host;
            client.Port = 587;   
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try { 
                client.Send(message);
            } catch (Exception ex)
            {
                var a = ex;
            }
        }

        private static MailMessage InitMailMessage(string ToEmail, string Subject, string Body)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(FromAddress);
            message.To.Add(new MailAddress(ToEmail));
            message.Subject = Subject;
            message.IsBodyHtml = true;
            message.Body = Body;
            return message;
        }

        private static void SendNameUrlEmail(string Layout, User User, string Url, string Subject)
        {
            var template = File.ReadAllText(Path.Combine(templateFolderPath, Layout));
            NameUrlViewModel model = new NameUrlViewModel
            {
                FirstName = User.FirstName,
                Url = Url
            };

            var body = Razor.Parse(template, model);
            Send(InitMailMessage(User.Email, Subject, body));
        }
        #endregion
    }
}