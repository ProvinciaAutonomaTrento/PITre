using System;
using DocsPaVO.ProfilazioneDinamica;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace DocsPaVO.Grid
{
    /// <summary>
    /// Questa classe rappresenta le impostazioni relative ad un campo di una griglia
    /// di ricerca.
    /// </summary>
    [Serializable()]
    public class Field
    {
        public Field()
        {
            this.IsTruncable = true;
            this.MaxLength = 100;
            this.Width = 100;
 
        }

        /// <summary>
        /// L'identificativo univoco del campo
        /// </summary>
        public string FieldId { get; set; }

        /// <summary>
        /// Il nome originale del campo
        /// </summary>
        public string OriginalLabel { get; set; }

        /// <summary>
        /// L'etichetta associata al campo.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Booleano utilizzato per indicare se il campo è un campo multivalore.
        /// </summary>
        public bool CanAssumeMultiValues { get; set; }

        /// <summary>
        /// Valore booleano utilizzato per indicare se la proprietà
        /// MaxLength può essere valorizzata
        /// </summary>
        public bool IsTruncable { get; set; }

        /// <summary>
        /// Il numero di caratteri massimi da visualizzare per questo campo.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Largezza del campo
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Valore che indica se il campo deve essere visualizzato
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Posizione del campo all'interno della griglia
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Identificativo univoco dell'oggetto custom associato a questo campo.
        /// 0 indica un campo non custom.
        /// </summary>
        public int CustomObjectId { get; set; }

        /// <summary>
        /// Nome del template cui appartiene questo campo.
        /// </summary>
        public String AssociatedTemplateName { get; set; }

        /// <summary>
        /// Id del template associato al campo.
        /// </summary>
        public String AssociatedTemplateId { get; set; }

        /// <summary>
        /// In caso di campo multivalore, lista dei valori associati
        /// </summary>
        [XmlArray()]
        [XmlArrayItem(typeof(FieldValue))]
        public List<FieldValue> Values { get; set; }
        
        /// <summary>
        /// Nome della colonna di db Oracle con cui mappare questo campo (viene utilizzato per
        /// le operazioni si ordinamento)
        /// </summary>
        public String OracleDbColumnName { get; set; }

        /// <summary>
        /// Nome della colonna di db SQL Server con cui mappare questo campo (viene utilizzato per
        /// le operazione si ordinamento)
        /// </summary>
        public String SqlServerDbColumnName { get; set; }

        /// <summary>
        /// True se il campo non è modificabile
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// True se il campo è un campo comune
        /// </summary>
        public bool IsCommonField { get; set; }

        /// <summary>
        /// True se il campo deve essere ordinato per numero
        /// </summary>
        public bool IsNumber { get; set; }

    }

}