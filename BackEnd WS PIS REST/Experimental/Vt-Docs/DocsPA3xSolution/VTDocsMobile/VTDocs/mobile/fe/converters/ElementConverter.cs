using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using VTDocsMobile.VTDocsWSMobile;

namespace VTDocs.mobile.fe.converters
{
    public delegate IDictionary<string,object> Serialize<C>(C element);

    public class ElementConverter<C> : JavaScriptConverter
    {
        private Serialize<C> _serialize; 

        public ElementConverter(Serialize<C> serialize)
        {
            this._serialize = serialize;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new InvalidOperationException("We only serialize");
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            C element =(C) obj;
            return _serialize(element);
        }

        public override IEnumerable<Type> SupportedTypes { get { return new Type[] { typeof(C) }; } }
    }

}