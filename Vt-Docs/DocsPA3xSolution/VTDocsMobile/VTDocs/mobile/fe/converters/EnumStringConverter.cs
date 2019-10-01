using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;

namespace VTDocs.mobile.fe.converters
{
    public class EnumStringConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            foreach (string key in dictionary.Keys)
            {
                try { return Enum.Parse(type, dictionary[key].ToString(), false); }
                catch (Exception ex) { throw new SerializationException("Problem trying to deserialize enum from JSON.", ex); }
            }
            return Activator.CreateInstance(type);
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> objs = new Dictionary<string, object>();
            objs.Add(obj.ToString(), ((Enum)obj).ToString("D"));
            return objs;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new Type[] { typeof(Enum) }; } }
    }

}