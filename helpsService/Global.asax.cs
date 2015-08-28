using System.Web.Http;
using System.Web.Routing;
using helps.Service;

namespace helps.Service
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register();
        }
    }
}