using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;
using System.Linq;
using System.Web.Script.Serialization;

namespace helps.Shared
{
    public class AuthService : Main
    {
        public AuthService()
        {
            Init();
        }

        public async Task<GenericResponse> Login(LoginRequest request)
        {
            try
            {   
                var response = await authClient.InvokeApiAsync<LoginRequest, User>("SignIn", request);
                database.SetUser(response);
                return Success();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse("Login Failure", ex);
            }
        }

        public User CurrentUser()
        {
            return database.CurrentUser();
        }

        public async Task<GenericResponse> Register(RegisterRequest request)
        {
            try
            {
                database.SetUser(new User { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, StudentId = request.StudentId });
                var response = await authClient.InvokeApiAsync<RegisterRequest, GenericResponse>("Registration", request);
                return Success();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse("Registration Failure", ex);
            }
        }

        public async Task<GenericResponse> ForgotPassword(string StudentId)
        {
            try
            {
                Dictionary<string, string> paramaters = new Dictionary<string, string>();
                paramaters.Add("studentId", StudentId);
                JToken response = await authClient.InvokeApiAsync("ForgotPassword", HttpMethod.Get, paramaters);
                return Success();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse("Failure", ex);
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
                return Success();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse("Failure", ex);
            }    
        }

        public async Task<GenericResponse> RegisterHelps(DetailsInputRequest request)
        {
            return Success();
        }

        public void Logout()
        {
            database.ClearCurrentUser();
        }
        
    }
}