using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using helps.Shared.Database;
using System.Net.Http;
using System.Net.Http.Headers;



namespace helps.Shared
{
    public class Main
    {

        public MobileServiceClient authClient;
        public HttpClient helpsClient;

        public helpsDatabase database;

        public const string servicesApplicationURL = @"http://54.153.240.143/";
        public const string servicesApplicationKey = @"EcJyqLPpfEiVHyiAwKGmrIKvCQXjtL23";

        public const string helpsApplicationURL = @"http://helps.pagnan.com.au/";
        public const string helpsApplicationKey = @"94n4NXGofY2Esdd36GlQ3JR66T102bXI";

        public async void Init()
        {
            //CurrentPlatform.Init();
            authClient = new MobileServiceClient(servicesApplicationURL, servicesApplicationKey);
            helpsClient = new HttpClient();
            helpsClient.BaseAddress = new Uri(helpsApplicationURL);
            helpsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            helpsClient.DefaultRequestHeaders.Add("AppKey", helpsApplicationKey);
            database = new helpsDatabase();            
        }
        public GenericResponse Success()
        {
            return new GenericResponse
            {
                Success = true
            };
        }
        public GenericResponse CreateErrorResponse(string Title, Exception ex)
        {
            return new GenericResponse
            {
                Success = false,
                Message = ex.Message,
                Title = Title
            };
        }

        public GenericResponse CreateErrorResponse(string Title, string Message)
        {
            return new GenericResponse
            {
                Success = false,
                Message = Message,
                Title = Title
            };
        }
    }
}
