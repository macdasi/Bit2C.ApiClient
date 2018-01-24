using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bit2C.API {
    public class Serializer {

        public static T Deserialize<T>(string json) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static object getValueFromObject(object obj, string propertyName) {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static void setValueFromObject(object obj, string propertyName, object value) {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite) {
                prop.SetValue(obj, value, null);
            }
        }


    }
}
