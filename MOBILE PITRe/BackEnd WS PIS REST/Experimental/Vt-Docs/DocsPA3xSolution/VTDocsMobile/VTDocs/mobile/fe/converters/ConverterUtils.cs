using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using VTDocsMobile.VTDocsWSMobile;

namespace VTDocs.mobile.fe.converters
{
    public class ConverterUtils
    {
        private static ConfigurationHandler _confHandler = new ConfigurationHandler();

        private static IDictionary<string, object> ToDoListSerialize(ToDoListElement element)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            result.Add("DataDoc", element.DataDoc);
            result.Add("HasWorkflow", element.HasWorkflow);
            result.Add("TipoProto", element.TipoProto);
            result.Add("Segnatura", element.Segnatura);
            result.Add("Id", element.Id);
            result.Add("IdTrasm", element.IdTrasm);
            result.Add("IdEvento", element.IdEvento);
            result.Add("Mittente", element.Mittente);
            result.Add("Note", format(element.Note));
            result.Add("Oggetto", format(element.Oggetto));
            result.Add("Ragione", element.Ragione);
            result.Add("Tipo", element.Tipo);
            // MEV disponibilità anteprima
            //result.Add("Extension", formatExtension (element.Extension));
            string[] extensionAndPreview = formatExtensionAndPreview(element.Extension);
            result.Add("Extension", extensionAndPreview[0]);
            result.Add("Anteprima", extensionAndPreview[1]);
            return result;
        }

        private static IDictionary<string, object> RicercaSerialize(RicercaElement element)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            result.Add("Id", element.Id);
            result.Add("Note", format(element.Note));
            result.Add("Oggetto", format(element.Oggetto));
            result.Add("Tipo", element.Tipo);
            result.Add("TipoProto", element.TipoProto);
            result.Add("Segnatura", element.Segnatura);
            //result.Add("Extension", formatExtension(element.Extension));
            // MEV disponibilità anteprima
            string[] extensionAndPreview = formatExtensionAndPreview(element.Extension);
            result.Add("Extension", extensionAndPreview[0]);
            result.Add("Anteprima", extensionAndPreview[1]);
            
            return result;
        }

        /// <summary>
        /// MEV Disponibilità anteprima
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string[] formatExtensionAndPreview(string extension)
        {
            string[] retVal = new string[2];

            if (string.IsNullOrEmpty(extension) || extension.Equals("0"))
            {
                // file non acquisito
                retVal[0] = "-";
                retVal[1] = "File non acquisito";
            }
            else
            {
                retVal[0] = extension;
                // file acquisito - estensione supportata
                if (extension.ToUpper().Equals("PDF"))
                {
                    retVal[1] = "Disponibile";
                }
                // file acquisito - estensione non supportata
                else
                {
                    retVal[1] = "Non disponibile";
                }
            }

            return retVal;
        }

        private static string formatExtension(string inExt)
        {
            if (string.IsNullOrEmpty(inExt))
                return null;

            string Ext = inExt.ToUpper();
            if (Ext == "0")
                return "Non Acquisito";
            if (Ext.Equals("PDF"))
            {
                Ext += " - Anteprima Disponibile";
                return Ext;
            }
            else
                return Ext;
        }
        private static string format(string value)
        {
            if (value == null) return "";
            if (value.Length <= _confHandler.MaxStringLength)
            {
                return value;
            }
            else
            {
                int firstSpacePos=value.IndexOf(' ', _confHandler.MaxStringLength);
                if (firstSpacePos == -1) firstSpacePos = _confHandler.MaxStringLength;
                return value.Substring(0, firstSpacePos) + "...";
            }
        }
        
        public static List<JavaScriptConverter> JavaScriptConverters {
            get{
               return new List<JavaScriptConverter>{
                   new EnumStringConverter(), 
                   new ElementConverter<ToDoListElement>(ToDoListSerialize), 
                   new ElementConverter<RicercaElement>(RicercaSerialize)
               };
            }
        }

    }
}