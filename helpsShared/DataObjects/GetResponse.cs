using System;
using System.Collections.Generic;
namespace helps.Shared.DataObjects
{
    public class GetResponse<T>
    {
        public List<T> Results { get; set; }
        public bool IsSuccess { get; set; }
        public string DisplayMessage { get; set; }
    }
}
