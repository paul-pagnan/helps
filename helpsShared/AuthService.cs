using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;
using System.Linq;
using helps.Shared.Helpers;

namespace helps.Shared
{
    public class AuthService : Main
    {
        public AuthService()
        {
            Init();
        }

        public async Task<AuthResult> Login(string StudentId, string Password)
        {
            AuthResult result;
            string input = "{ 'studentId': '" + StudentId + "', 'password': '" + Password + "'}";

            try
            {   
                var response = await client.InvokeApiAsync("SignIn", input);

                //JSONHelper<LoginResponse>.DeSerialize(response.);

                result = new AuthResult { Success = true };
            }
            catch (Exception ex)
            {
                result = new AuthResult
                {
                    Success = false,
                    Message = ex.Message,
                    Title = "Login Failure"
                };
            }
            return result;
        }

        public User Current_User()
        {
            return database.GetCurrentUser();
        }

        public async Task<AuthResult> Register(string FirstName, string LastName, string Email, string StudentId, string Password)
        {
            AuthResult result;
            string input = "{ 'firstName': '" + FirstName + "', 'lastName': '" + LastName + "','studentId': '" + StudentId + "', 'password': '" + Password + "', 'email': '" + Email + "'}";
            try
            {
                database.SetUser(new User { FirstName = FirstName, LastName = LastName, Email = Email });
                JToken response = await client.InvokeApiAsync("Registration", input);
                result = new AuthResult { Success = true };
            }
            catch (Exception ex)
            {
                result = new AuthResult
                {
                    Success = false,
                    Message = ex.Message,
                    Title = "Registration Failure"
                };
            }
            return result;
        }

        public async Task<AuthResult> ForgotPassword(string StudentId)
        {
            AuthResult result;
            Dictionary<string, string> paramaters = new Dictionary<string, string>();
            paramaters.Add("studentId", StudentId);
            try
            {
                JToken response = await client.InvokeApiAsync("ForgotPassword", HttpMethod.Get, paramaters);
                result = new AuthResult { Success = true };
            }
            catch (Exception ex)
            {
                result = new AuthResult
                {
                    Success = false,
                    Message = ex.Message,
                    Title = "Failure"
                };
            }
            return result;
        }

        public async Task<AuthResult> ResendConfirmation(string StudentId)
        {
            AuthResult result;
            Dictionary<string, string> paramaters = new Dictionary<string, string>();
            paramaters.Add("StudentId", StudentId);
            paramaters.Add("Resend", "true");
            try
            {
                JToken response = await client.InvokeApiAsync("ConfirmEmail", HttpMethod.Get, paramaters);
                result = new AuthResult { Success = true };
            }
            catch (Exception ex)
            {
                result = new AuthResult
                {
                    Success = false,
                    Message = ex.Message,
                    Title = "Failure"
                };
            }
            return result;
        }
    }
}