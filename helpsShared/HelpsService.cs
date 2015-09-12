using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

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
            try
            {
                var response = await helpsClient.InvokeApiAsync<HelpsRegisterRequest, HelpsResponse>("student/register", request);
                return Success();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse("Registration Failed", ex);
            }
        }
    }
}
