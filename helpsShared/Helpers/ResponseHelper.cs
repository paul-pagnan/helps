using helps.Shared.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helps.Shared.Helpers
{
    public class ResponseHelper
    {
        public static GenericResponse Success()
        {
            return new GenericResponse
            {
                Success = true
            };
        }
        public static GenericResponse CreateErrorResponse(string Title, Exception ex)
        {
            return new GenericResponse
            {
                Success = false,
                Message = ex.Message,
                Title = Title
            };
        }

        public static GenericResponse CreateErrorResponse(string Title, string Message)
        {
            return new GenericResponse
            {
                Success = false,
                Message = Message,
                Title = Title
            };
        }
    }
}
