// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using SoapCore.Serializer;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace Pi3.App.Legacy.Admin.WebApi
{
    public class CustomSerializer : IXmlSerializationHandler
    {
        public object DeserializeInputParameter(XmlDictionaryReader xmlReader, Type parameterType, string parameterName, string parameterNs, ICustomAttributeProvider customAttributeProvider, IEnumerable<Type> knownTypes = null)
        {
            var serializer = new DataContractSerializer(parameterType, parameterName, parameterNs, (IEnumerable<Type>)knownTypes);
            return serializer.ReadObject(xmlReader, true);
        }
    }
}
