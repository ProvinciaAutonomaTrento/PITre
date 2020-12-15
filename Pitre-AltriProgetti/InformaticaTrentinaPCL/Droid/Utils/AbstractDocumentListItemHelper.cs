using System;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using Java.Lang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    public class AbstractDocumentListItemHelper
    {
        public static AbstractDocumentListItem DeserializeAbstractDocumentListItem(string serializedIDocumentListItem)
        {
            JsonConverter[] converters = { new AbstractDocumentListItemJsonConverter()};
            AbstractDocumentListItem abstractDocumentListItem = JsonConvert.DeserializeObject<AbstractDocumentListItem>(
                serializedIDocumentListItem, 
                new JsonSerializerSettings() { Converters = converters });

            return abstractDocumentListItem;
        }

    }
    
    class AbstractDocumentListItemJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AbstractDocumentListItem);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(ToDoDocumentModel))
                return jo.ToObject<ToDoDocumentModel>(serializer);

            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(SignDocumentModel))
                return jo.ToObject<SignDocumentModel>(serializer);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(RicercaDocumentModel))
                return jo.ToObject<RicercaDocumentModel>(serializer);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(AdlDocumentModel))
                return jo.ToObject<AdlDocumentModel>(serializer);
            
            //TODO CustomJsonConverter: le nuovi classi che estendono AbstractRecipient, vanno aggiunte qui per la serializzazione e deserializzazione.
            
            throw new RuntimeException("CustomJsonConverter.ReadJson - Concrete class not found");
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        
    }
}