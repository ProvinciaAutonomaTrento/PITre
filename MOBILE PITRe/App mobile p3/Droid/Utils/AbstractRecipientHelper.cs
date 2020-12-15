using System;
using InformaticaTrentinaPCL.Assegna;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;
using Java.Lang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InformaticaTrentinaPCL.Droid.Utils
{
    public class AbstractRecipientHelper
    {
        public static AbstractRecipient DeserializeAbstractRecipient(string serializedRecipient)
        {
            JsonConverter[] converters = { new CustomJsonConverter()};
            AbstractRecipient recipient = JsonConvert.DeserializeObject<AbstractRecipient>(
                serializedRecipient, 
                new JsonSerializerSettings() { Converters = converters });

            return recipient;
        }

    }
    
    class CustomJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AbstractRecipient);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(AssigneeModel))
                return jo.ToObject<AssigneeModel>(serializer);

            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(SearchMandateAssignee))
                return jo.ToObject<SearchMandateAssignee>(serializer);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(InfoPreferito))
                return jo.ToObject<InfoPreferito>(serializer);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(RuoloInfo))
                return jo.ToObject<RuoloInfo>(serializer);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(CorrispondenteTrasmissioneModel))
                return jo.ToObject<CorrispondenteTrasmissioneModel>(serializer);
            
            if (Type.GetType(jo["InstanceType"].ToString()) == typeof(ModelliTrasmissioneModel))
                return jo.ToObject<ModelliTrasmissioneModel>(serializer);
            
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