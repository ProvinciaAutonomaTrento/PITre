// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Text.Json.Serialization;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace Pi3.Core.SeedWork
//{
//    public class AggregateRootJsonConverter<A, K> : JsonConverter<A> where A : AggregateRoot<K>
//    {
//        public override A? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//        {
//            var instance = AggregateRootFactory.Create<A, K>();

//            var containers = JsonSerializer.Deserialize<List<EventContainer>>(ref reader, options);

//            var events = new List<IEvent>();

//            containers.ForEach(c =>
//                events.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<IEvent>(c.Data, new Newtonsoft.Json.JsonSerializerSettings()
//                {
//                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
//                })));


//            instance.LoadFromHistory(events.ToArray());

//            return instance;
//        }

//        public override void Write(Utf8JsonWriter writer, A value, JsonSerializerOptions options)
//        {
//            var containers = value.GetUncommittedChanges()?.Concat(value.GetCommittedChanges())
//                .OfType<IEvent>()
//                .Select(e => new EventContainer()
//                {
//                    Name = e.GetType().Name,
//                    Type = e.GetType().AssemblyQualifiedName,
//                    Data = Newtonsoft.Json.JsonConvert.SerializeObject(
//                            e,
//                            Newtonsoft.Json.Formatting.Indented,
//                            new Newtonsoft.Json.JsonSerializerSettings()
//                            {
//                                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
//                            })
//                })
//                .ToList();

//            writer.WriteRawValue(JsonSerializer.Serialize<List<EventContainer>>(containers, options));
//        }

//        protected class EventContainer
//        {
//            public string Name { get; set; }

//            public string Type { get; set; }

//            public string Data { get; set; }
//        }
//    }
//}
