using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

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

        public async Task<AuthResult> Register(string FirstName, string LastName, string StudentId, string Password)
        {
            AuthResult result;
            string input = "{ 'firstName': '" +  FirstName + "', 'lastName': '" + LastName + "','studentId': '" + StudentId + "', 'password': '" + Password + "'}";
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
    }
}