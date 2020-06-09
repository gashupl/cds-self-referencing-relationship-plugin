using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SelfReferencingRelationshipPlugin.Internal
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string json)
        {
            using(var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream); 
            }

        }
    }
}
