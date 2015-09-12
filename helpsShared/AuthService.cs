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

        public async Task<AuthResult> Login(LoginRequest request)
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

        public async Task<AuthResult> Register(RegisterRequest request)
        {
            try
            {
                database.SetUser(new User { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, StudentId = request.StudentId });
                var response = await authClient.InvokeApiAsync<RegisterRequest, DefaultResponse>("Registration", request);
                return Success();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse("Registration Failure", ex);
            }
        }

        public async Task<AuthResult> ForgotPassword(string StudentId)
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

        public async Task<AuthResult> ResendConfirmation(string StudentId)
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

        public async Task<AuthResult> RegisterHelps(DetailsInputRequest request)
        {

            return Success();
        }

        private AuthResult Success()
        {
            return new AuthResult {
                Success = true
            };
        }
        private AuthResult CreateErrorResponse(string Title, Exception ex)
        {
            return new AuthResult
            {
                Success = false,
                Message = ex.Message,
                Title = Title
            };
        }
    }
}