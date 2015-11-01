using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using helps.Shared.Helpers;
using SQLite;

namespace helps.Shared
{
    public class StudentService : HelpsService
    {
        public StudentService() : base()
        {            
        }

        public async Task<GenericResponse> RegisterStudent(HelpsRegisterRequest request)
        {
            var response = helpsClient.PostAsJsonAsync("api/student/register", request).Result;

            if (response.IsSuccessStatusCode) {
                HelpsResponse decodedResponse = response.Content.ReadAsAsync<HelpsResponse>().Result;
                if (decodedResponse.IsSuccess)
                {
                    var user = userTable.CurrentUser();
                    user.HasLoggedIn = true;
                    AuthService auth = new AuthService();
                    GenericResponse Response = await auth.CompleteSetup(userTable.CurrentUser().StudentId);
                    if (Response.Success)
                    {
                        userTable.SetUser(user);
                        return ResponseHelper.Success();
                    }
                }
                else
                    return ResponseHelper.CreateErrorResponse("Registration Failed", decodedResponse.DisplayMessage);
            }
            return ResponseHelper.CreateErrorResponse("Registration Failed", "An unknown error occurred");            
        }

        public async Task<Student> GetStudent()
        {
            var response = await helpsClient.GetAsync("api/student?studentId=" + AuthService.GetCurrentUser().StudentId);
            if (response.IsSuccessStatusCode)
            {
                var decodedResponse = await response.Content.ReadAsAsync<StudentResponse>();
                if (decodedResponse.IsSuccess)
                    return decodedResponse.Student;
            }
            return null;
        }
    }
}
