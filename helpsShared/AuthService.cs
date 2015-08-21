using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace helps.Shared
{
    public class AuthService : Main
    {
        public AuthService()
        {
            Init();
        }

        public async Task<JToken> Login(string StudentId, string Password)
        {
            string input = "{ 'studentId': '" + StudentId + "', 'password': '" + Password + "'}";
            return await client.InvokeApiAsync("SignIn", input);
        }
    }
}

