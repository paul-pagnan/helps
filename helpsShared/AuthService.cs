using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;
using System.Linq;
using System.Web.Script.Serialization;
using helps.Shared.Database;
using Microsoft.WindowsAzure.MobileServices;
using helps.Shared.Helpers;

namespace helps.Shared
{
    public class AuthService : Main
    {
        public const string servicesApplicationURL = @"http://54.153.240.143/";
        public const string servicesApplicationKey = @"EcJyqLPpfEiVHyiAwKGmrIKvCQXjtL23";
        public MobileServiceClient authClient;

        public AuthService() : base()
        {
            authClient = new MobileServiceClient(servicesApplicationURL, servicesApplicationKey);
        }

        public async Task<GenericResponse> Login(LoginRequest request)
        {
            try
            {   
                var response = await authClient.InvokeApiAsync<LoginRequest, User>("SignIn", request);
                userTable.SetUser(response);
                return ResponseHelper.Success();
            }
            catch (Exception ex)
            {
                return ResponseHelper.CreateErrorResponse("Login Failure", ex);
            }
        }

        public User CurrentUser()
        {
            return userTable.CurrentUser();
        }

        public static User GetCurrentUser()
        {
            return userTable.CurrentUser();
        }

        public async Task<GenericResponse> Register(RegisterRequest request)
        {
            try
            {
                userTable.SetUser(new User { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, StudentId = request.StudentId });
                var response = await authClient.InvokeApiAsync<RegisterRequest, GenericResponse>("Registration", request);
                return ResponseHelper.Success();
            }
            catch (Exception ex)
            {
                return ResponseHelper.CreateErrorResponse("Registration Failure", ex);
            }
        }

        public async Task<GenericResponse> ForgotPassword(string StudentId)
        {
            try
            {
                Dictionary<string, string> paramaters = new Dictionary<string, string>();
                paramaters.Add("studentId", StudentId);
                JToken response = await authClient.InvokeApiAsync("ForgotPassword", HttpMethod.Get, paramaters);
                return ResponseHelper.Success();
            }
            catch (Exception ex)
            {
                return ResponseHelper.CreateErrorResponse("Failure", ex);
            }
        }

        public async Task<GenericResponse> ResendConfirmation(string StudentId)
        {
            try
            {
                Dictionary<string, string> paramaters = new Dictionary<string, string>();
                paramaters.Add("StudentId", StudentId);
                paramaters.Add("Resend", "true");
                JToken response = await authClient.InvokeApiAsync("ConfirmEmail", HttpMethod.Get, paramaters);
                return ResponseHelper.Success();
            }
            catch (Exception ex)
            {
                return ResponseHelper.CreateErrorResponse("Failure", ex);
            }    
        }

        public async Task<GenericResponse> CompleteSetup(string StudentId)
        {
            try
            {
                Dictionary<string, string> paramaters = new Dictionary<string, string>();
                paramaters.Add("StudentId", StudentId);
                var response = await authClient.InvokeApiAsync("CompleteSetup", HttpMethod.Get, paramaters);
                return ResponseHelper.Success();
            }
            catch (Exception ex)
            {
                return ResponseHelper.CreateErrorResponse("Failure", ex);
            }
        }

        public GenericResponse RegisterHelps(DetailsInputRequest request)
        {
            return ResponseHelper.Success();
        }

        public void Logout()
        {
            userTable.ClearCurrentUser();
            helpsDatabase.ClearDatabase();
        }
        
    }
}