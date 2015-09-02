using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace helps.Shared.Helpers
{
    public static class JSONHelper<TType> where TType : class
    {
        /// <summary>
        /// Serializes an object to JSON
        /// </summary>
        //public static string Serialize(TType instance)
        //{
        //    var serializer = new DataContractJsonSerializer(typeof(TType));
        //    using (var stream = new MemoryStream())
        //    {
        //        serializer.WriteObject(stream, instance);
        //        return Encoding.UTF8.GetString(stream.ToArray());
        //    }
        //}

        /// <summary>
        /// DeSerializes an object from JSON
        /// </summary>
        public static TType DeSerialize(string json)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(TType));
                return serializer.ReadObject(stream) as TType;
            }
        }
    }
}
