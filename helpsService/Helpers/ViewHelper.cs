using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace helps.Service.Helpers
{
    public static class ViewHelper
    {
        private static string templateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views");
        public static HttpResponseMessage View(string Layout, dynamic Model = null)
        {
            var template = File.ReadAllText(Path.Combine(templateFolderPath, Layout + ".cshtml"));
            var response = new HttpResponseMessage();
            response.Content = new StringContent(Razor.Parse(template, Model));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
