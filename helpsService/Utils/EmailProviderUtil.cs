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


        public static string SendConfirmationEmail(User user, string url)
        {
            var body = "";
            try {
                var config = new TemplateServiceConfiguration();
                config.EncodedStringFactory = new RawStringFactory();

                var configuredTemplateService = new TemplateService(config);
                Razor.SetTemplateService(configuredTemplateService);

                var template = File.ReadAllText(Path.Combine(templateFolderPath, "ConfirmEmail.cshtml"));
                ConfirmEmailViewModel model = new ConfirmEmailViewModel
                {
                    FirstName = user.FirstName,
                    Url = url
                };

                body = Razor.Parse("safasfasfas");
            } catch (Exception ex)
            {
                return ex.ToString();
            }
            return body;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(FromAddress);
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "UTS HELPS - Please confirm your email";
            message.IsBodyHtml = true;
            message.Body = body;

            Send(message);
        }

        public static void SendConfirmationEmailOld(User user, string url)
        {
            var template = File.ReadAllText(Path.Combine(templateFolderPath, "ConfirmEmail.html"));
            var body = string.Format(template, user.FirstName, url, url);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(FromAddress);
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "UTS HELPS - Please confirm your email";
            message.IsBodyHtml = true;
            message.Body = body;

            Send(message);
        }

        public static void SendPasswordResetEmail(User user, string url)
        {
            var template = File.ReadAllText(Path.Combine(templateFolderPath, "ResetPassword.html"));
            var body = string.Format(template, user.FirstName, url, url);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(FromAddress);
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "UTS HELPS - Password reset";
            message.IsBodyHtml = true;
            message.Body = body;

            Send(message);
        }


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
    }
}