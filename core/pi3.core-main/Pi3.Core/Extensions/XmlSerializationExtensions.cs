// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Pi3.Core.Extensions
{
    // <summary>
    /// Overrides the base 'StringWriter' class to accept a different character encoding type.
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        #region Properties

        /// <summary>
        /// Overrides the default encoding type (UTF-16).
        /// </summary>
        public override Encoding Encoding => _encoding ?? base.Encoding;

        #endregion

        #region Readonlys

        private readonly Encoding _encoding;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StringWriterWithEncoding() { }

        /// <summary>
        /// Constructor which accepts a character encoding type.
        /// </summary>
        /// <param name="encoding">The character encoding type</param>
        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        #endregion
    }

    public static class XmlSerializationExtensions
    {
        /// <summary>
        /// Converts an object to its serialized XML format.
        /// </summary>
        /// <typeparam name="T">The type of object we are operating on</typeparam>
        /// <param name="value">The object we are operating on</param>
        /// <param name="indent"></param>
        /// <param name="removeDefaultXmlNamespaces">Whether or not to remove the default XML namespaces from the output</param>
        /// <param name="omitXmlDeclaration">Whether or not to omit the XML declaration from the output</param>
        /// <param name="encoding">The character encoding to use</param>
        /// <returns>The XML string representation of the object</returns>
        public static string ToXmlString<T>(this T value, bool indent = false, bool removeDefaultXmlNamespaces = true, bool omitXmlDeclaration = true, Encoding encoding = null) where T : class
        {
            XmlSerializerNamespaces namespaces = removeDefaultXmlNamespaces ? new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }) : null;

            var settings = new XmlWriterSettings();
            settings.Indent = indent;
            settings.OmitXmlDeclaration = omitXmlDeclaration;
            settings.CheckCharacters = false;

            using (var stream = new StringWriterWithEncoding(encoding))
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, namespaces);
                return stream.ToString();
            }
        }

        /// <summary>
        /// Creates an object instance from the specified XML string.
        /// </summary>
        /// <typeparam name="T">The Type of the object we are operating on</typeparam>
        /// <param name="value">The object we are operating on</param>
        /// <param name="xml">The XML string to deserialize from</param>
        /// <returns>An object instance</returns>
        public static T FromXmlString<T>(this T value, string xml) where T : class
        {
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
