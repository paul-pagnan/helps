using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

namespace helps.Shared
{
    public class HelpsService : Main
    {
        public HelpsService()
        {
            Init();
        }

        public async Task<GenericResponse> RegisterStudent(HelpsRegisterRequest request)
        {
            var response = helpsClient.PostAsJsonAsync("api/student/register", request).Result;

            if (response.IsSuccessStatusCode) {
                HelpsResponse decodedResponse = response.Content.ReadAsAsync<HelpsResponse>().Result;
                if (decodedResponse.IsSuccess)
                    return Success();
                else
                    return CreateErrorResponse("Registration Failed", decodedResponse.DisplayMessage);
            }
            return CreateErrorResponse("Registration Failed", "An unknown error occurred");            
        }
    }
}
