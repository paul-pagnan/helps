using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;


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
                await client.InvokeApiAsync("SignIn", input);
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

        public async Task<AuthResult> Register(string FirstName, string LastName, string Email, string StudentId, string Password)
        {
            AuthResult result;
            string input = "{ 'firstName': '" + FirstName + "', 'lastName': '" + LastName + "','studentId': '" + StudentId + "', 'password': '" + Password + "', 'email': '" + Email + "'}";
            try
            {
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
    }
}