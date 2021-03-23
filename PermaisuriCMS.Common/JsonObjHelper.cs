using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PermaisuriCMS.Common
{
    public class JsonObjHelper
    {
        public static string Obj2Json(object obj)
        {
            string result = "";
            if (obj == null)
            {
                return result;
            }
            var serializer = new JavaScriptSerializer();
            result = serializer.Serialize(obj);
            return result;
        }


        public static object Json2Obj(string json, Type t)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            object result = null;
            try
            {
                var serializer = new JavaScriptSerializer();
                result = serializer.Deserialize(json, t);
            }
            catch
            {

            }

            return result;
        }
    }
}
