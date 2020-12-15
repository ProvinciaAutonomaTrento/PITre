using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DocsPaVO.documento
{
    /// <summary>
    /// Attributo di utilità da associare agli enumerations per determinare facilmente lo stato di consolidamento
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DocumentConsolidationStateDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public DocumentConsolidationStateDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Stato di consolidamento del documento
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DocumentConsolidationStateDescriptionAttribute[] attributes = (DocumentConsolidationStateDescriptionAttribute[])
                    fi.GetCustomAttributes(typeof(DocumentConsolidationStateDescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return string.Empty;
        }
    }

    /// <summary>
    /// Possibili stati di consolidamento cui può venirsi a trovare un documento
    /// </summary>
    public enum DocumentConsolidationStateEnum
    {
        /// <summary>
        /// Documento ancora non consolidato
        /// </summary>
        [DocumentConsolidationStateDescription("Non consolidato")]
        None = 0,

        /// <summary>
        /// Consolidato il documento in azioni fondamentali che determinano un incremento o modifica delle versioni
        /// </summary>
        [DocumentConsolidationStateDescription("Consolidato contenuto")]
        Step1 = 1,

        /// <summary>
        /// Consolidato il documento nei suoi metadati fondamentali
        /// </summary>
        [DocumentConsolidationStateDescription("Consolidato contenuto e metadati")]
        Step2 = 2,
    }

    /// <summary>
    /// Informazioni di stato di consolidamento del documento
    /// </summary>
    [Serializable()]
    public class DocumentConsolidationStateInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public DocumentConsolidationStateInfo()
        {
            this.State = DocumentConsolidationStateEnum.None;
        }

        /// <summary>
        /// Stato di consolidamento del documento
        /// </summary>
        public DocumentConsolidationStateEnum State { get; set; }

        /// <summary>
        /// Utente che ha consolidato il documento
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Ruolo dell'utente che ha consolidato il documento
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Data in cui il documento è stato consolidato
        /// </summary>
        public string Date { get; set; }
    }
}